#if GBG_HIDDEN_OBJECT_FINDER_DEV
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectFinderTesterWindow : EditorWindow
    {
        [MenuItem("Tools/Bamboo/Hidden Object Finder Tester")]
        public static void Open()
        {
            GetWindow<HiddenObjectFinderTesterWindow>("Hidden Object Finder Tester").Focus();
        }


        private UObject _targetObj;
        private HideFlags _newHideFlags;


        private void OnEnable()
        {
            minSize = new Vector2(300, 100);
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            _targetObj = EditorGUILayout.ObjectField("Object", _targetObj, typeof(UObject), true);
            EditorGUILayout.EnumFlagsField("Object Hide Flags", _targetObj ? _targetObj.hideFlags : HideFlags.None);

            EditorGUILayout.Space();
            bool targetObjModifiable = _targetObj && !EditorUtility.IsPersistent(_targetObj);
            EditorGUI.BeginDisabledGroup(!targetObjModifiable);
            {
                _newHideFlags = (HideFlags)EditorGUILayout.EnumFlagsField("New Hide Flags", _newHideFlags);

                EditorGUILayout.Space();
                if (GUILayout.Button("Set New Hide Flags"))
                {
                    Undo.RecordObject(_targetObj, "Set HideFlags");
                    _targetObj.hideFlags = _newHideFlags;
                    EditorUtility.SetDirty(_targetObj);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif