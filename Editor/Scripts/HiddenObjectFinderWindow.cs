using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectFinderWindow : EditorWindow, IHasCustomMenu
    {
        #region Static

        [MenuItem("Tools/Bamboo/Hidden Object Finder")]
        public static void Open()
        {
            GetWindow<HiddenObjectFinderWindow>().Focus();
        }

        #endregion


        [SerializeField]
        private ObjectTypes _objectTypes = (ObjectTypes)~0;
        [SerializeField]
        private HideFlags _hideFlagsFilter = HiddenObjectFinder.DefaultHideFlagsFilter;
        private List<HierarchyItem> _rootHierarchyItems = new List<HierarchyItem>();
        [SerializeField]
        private TreeViewState _treeViewState;
        private HierarchyTreeViewElement _treeView;
        private HiddenObjectInspectorElement _inspector;


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
            titleContent = EditorGUIUtility.IconContent(
                EditorGUIUtility.isProSkin ? "d_SceneViewVisibility" : "SceneViewVisibility");
            titleContent.text = "Hidden Object Finder";
            minSize = new Vector2(600, 280);
            _treeViewState = _treeViewState ?? new TreeViewState();
        }

        private void CreateGUI()
        {
            #region Top

            VisualElement topContainer = new VisualElement
            {
                name = "TopContainer",
                style =
                {
                    flexShrink = 0,
                }
            };
            rootVisualElement.Add(topContainer);

            EnumFlagsField objectTypesField = new EnumFlagsField("Object Types", _objectTypes)
            {
                name = "ObjectTypesField",
                bindingPath = nameof(_objectTypes),
            };
            topContainer.Add(objectTypesField);

            EnumFlagsField enumFlagsField = new EnumFlagsField("Hide Flags Filter", _hideFlagsFilter)
            {
                name = "HideFlagsFilterField",
                bindingPath = nameof(_hideFlagsFilter),
            };
            topContainer.Add(enumFlagsField);

            Button findButton = new Button(OnFindButtonClicked)
            {
                name = "FindButton",
                text = "Find",
            };
            topContainer.Add(findButton);

            #endregion


            VisualElement contentContainer = new VisualElement
            {
                name = "ContentContainer",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };
            rootVisualElement.Add(contentContainer);


            #region Hierarchy Tree View

            _treeView = new HierarchyTreeViewElement(_treeViewState)
            {
                name = "HierarchyTreeViewContainer",
                style =
                {
                    width = 200,
                }
            };
            _treeView.HierarchyTreeView.OnClickItem += OnClickHiddenObject;
            contentContainer.Add(_treeView);

            #endregion


            #region Inspector

            _inspector = new HiddenObjectInspectorElement
            {
                name = "InspectorContainer",
                style =
                {
                    flexGrow = 1,
                }
            };
            contentContainer.Add(_inspector);

            #endregion


            #region Bind Properties

            rootVisualElement.Bind(new SerializedObject(this));

            #endregion
        }

        #endregion


        private void OnFindButtonClicked()
        {
            _inspector.SetTarget(null);
            FindHiddenObjects();
            RebuildHierarchyTreeView();
        }

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
                    hiddenObjList = HiddenObjectFinder.FindAll<Component>(_hideFlagsFilter).ConvertAll(comp => (UObject)comp);
                    break;

                case ObjectTypes.Other:
                    hiddenObjList = HiddenObjectFinder.FindAll(false, _hideFlagsFilter).Where(obj => !(obj is GameObject)).ToList();
                    break;

                // Mixed types
                default:
                    bool includeComponent = (_objectTypes & ObjectTypes.Component) != 0;
                    IEnumerable<UObject> tempObjs = HiddenObjectFinder.FindAll(includeComponent, _hideFlagsFilter);
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
            _treeViewState.lastClickedID = -1;
            _treeViewState.selectedIDs.Clear();
            _treeViewState.expandedIDs.Clear();
            _treeView.ClearItems();

            Dictionary<HierarchyItem, HierarchyTreeViewItem> registry = new Dictionary<HierarchyItem, HierarchyTreeViewItem>();
            foreach (HierarchyItem hierarchyItem in _rootHierarchyItems)
            {
                CreateTree(_treeView.RootItem, hierarchyItem, 0, registry);
            }

            _treeView.Reload();
            _treeView.ExpandAll();
        }

        private void CreateTree(TreeViewItem parent, HierarchyItem hierarchyItem, int depth,
            Dictionary<HierarchyItem, HierarchyTreeViewItem> registry)
        {
            HierarchyTreeViewItem treeViewItem = new HierarchyTreeViewItem(_treeView.GetUniqueItemId(), depth, hierarchyItem);

            parent.AddChild(treeViewItem);

            for (int i = hierarchyItem.Children.Count - 1; i >= 0; i--)
            {
                HierarchyItem childHierarchyItem = hierarchyItem.Children[i];
                CreateTree(treeViewItem, childHierarchyItem, depth + 1, registry);
            }
        }

        private void OnClickHiddenObject(HierarchyTreeViewItem item)
        {
            UObject selectedObject = item.HierarchyItem.Object;
            _inspector.SetTarget(selectedObject);
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
