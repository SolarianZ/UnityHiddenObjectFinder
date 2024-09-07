using PackagesGBG.HiddenObjectFinder.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectFinderWindow : EditorWindow, IHasCustomMenu
    {
        #region Static

        [MenuItem("Tools/Bamboo/Hidden Object Finder")]
        public static void Open()
        {
            GetWindow<HiddenObjectFinderWindow>("Hidden Object Finder").Focus();
        }

        #endregion


        [SerializeField]
        private ObjectTypes _objectTypes = (ObjectTypes)~0;
        [SerializeField]
        private HideFlags _hideFlagsFilter = HiddenObjectFinder.DefaultHideFlagsFilter;
        private List<HierarchyItem> _rootHierarchyItems = new List<HierarchyItem>();
        [SerializeField]
        private TreeViewState _treeViewState;
        private HierarchyTreeView _hierarchyTreeView;


        #region Unity Messages

        private void ShowButton(Rect position)
        {
            if (GUI.Button(position, EditorGUIUtility.IconContent("_Help"), GUI.skin.FindStyle("IconButton")))
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityHiddenObjectFinder");
            }
        }

        private void OnEnable()
        {
            _treeViewState = _treeViewState ?? new TreeViewState();
            _hierarchyTreeView = new HierarchyTreeView(_treeViewState);
        }

        private void OnGUI()
        {
            _objectTypes = (ObjectTypes)EditorGUILayout.EnumFlagsField("Object Types", _objectTypes);
            _hideFlagsFilter = (HideFlags)EditorGUILayout.EnumFlagsField("Hide Flags Filter", _hideFlagsFilter);

            EditorGUILayout.Space();
            if (GUILayout.Button("Find"))
            {
                FindHiddenObjects();
                RebuildHierarchyTreeView();
            }

            Rect usedRect = GUILayoutUtility.GetLastRect();
            _hierarchyTreeView.OnGUI(new Rect(usedRect.xMin, usedRect.yMax, position.width, position.height - usedRect.yMax));
        }

        #endregion


        private void FindHiddenObjects()
        {
            _rootHierarchyItems.Clear();

            if (_objectTypes == 0)
            {
                return;
            }

            IList<UObject> hiddenObjList;
            switch (_objectTypes)
            {
                case ObjectTypes.GameObject:
                    hiddenObjList = HiddenObjectFinder.FindGameObjects(_hideFlagsFilter).ConvertAll(go => (UObject)go);
                    break;

                case ObjectTypes.Component:
                    hiddenObjList = HiddenObjectFinder.FindAll<Component>().ConvertAll(comp => (UObject)comp);
                    break;

                case ObjectTypes.Other:
                    hiddenObjList = HiddenObjectFinder.FindAll(false).Where(obj => !(obj is GameObject)).ToList();
                    break;

                // Mixed types
                default:
                    bool includeComponent = (_objectTypes & ObjectTypes.Component) != 0;
                    IEnumerable<UObject> tempObjs = HiddenObjectFinder.FindAll(includeComponent);
                    bool includeGameObject = (_objectTypes & ObjectTypes.GameObject) != 0;
                    if (!includeGameObject)
                    {
                        tempObjs = tempObjs.Where(obj => !(obj is GameObject));
                    }
                    hiddenObjList = tempObjs.ToList();
                    break;
            }

            _rootHierarchyItems = HierarchyItem.BuildHierarchy(hiddenObjList);
        }

        private void RebuildHierarchyTreeView()
        {
            _hierarchyTreeView.ClearItems();

            Dictionary<HierarchyItem, TreeViewItem> registry = new Dictionary<HierarchyItem, TreeViewItem>();
            foreach (HierarchyItem hierarchyItem in _rootHierarchyItems)
            {
                CreateTree(_hierarchyTreeView.Root, hierarchyItem, 0, registry);
            }

            _hierarchyTreeView.Reload();
        }

        private void CreateTree(TreeViewItem parent, HierarchyItem hierarchyItem, int depth,
            Dictionary<HierarchyItem, TreeViewItem> registry)
        {
            TreeViewItem treeViewItem = new TreeViewItem(_hierarchyTreeView.GetUniqueItemId(), depth, hierarchyItem.GetDisplayName());
            parent.AddChild(treeViewItem);

            foreach (HierarchyItem childHierarchyItem in hierarchyItem.Children)
            {
                CreateTree(treeViewItem, childHierarchyItem, depth + 1, registry);
            }
        }


        #region IHasCustomMenu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // Source Code
            menu.AddItem(new GUIContent("Source Code"), false, () =>
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityHiddenObjectFinder");
            });
            menu.AddSeparator("");
        }

        #endregion


        readonly struct ObjectPathPartPair
        {
            public readonly UObject Object;
            public readonly List<string> PartPaths;

            public ObjectPathPartPair(UObject obj, List<string> partPaths)
            {
                Object = obj;
                PartPaths = partPaths;
            }
        }

        readonly struct ObjectPathPair : IEquatable<ObjectPathPair>
        {
            public readonly UObject Obj;
            public readonly string Path;

            public ObjectPathPair(UObject obj, string path)
            {
                Obj = obj;
                Path = path;
            }

            public override bool Equals(object obj)
            {
                return obj is ObjectPathPair key && Equals(key);
            }

            public bool Equals(ObjectPathPair other)
            {
                return Path == other.Path &&
                       Obj == other.Obj;
            }

            public static bool operator ==(ObjectPathPair left, ObjectPathPair right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ObjectPathPair left, ObjectPathPair right)
            {
                return !(left == right);
            }

            public override int GetHashCode()
            {
                int hashCode = 588422528;
                hashCode = hashCode * -1521134295 + EqualityComparer<UObject>.Default.GetHashCode(Obj);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
                return hashCode;
            }
        }
    }
}
