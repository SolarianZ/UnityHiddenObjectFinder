using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectInspectorElement : VisualElement
    {
        private InspectorElement _inspector;
        private UObject _object;
        private HideFlags _newHideFlags;


        public HiddenObjectInspectorElement()
        {
            IMGUIContainer operationDrawer = new IMGUIContainer(DrawObjectOperations);
            Add(operationDrawer);
        }

        private void DrawObjectOperations()
        {
            EditorGUILayout.EnumFlagsField("Current Hide Flags", _object ? _object.hideFlags : HideFlags.None);

            bool targetObjModifiable = _object && !EditorUtility.IsPersistent(_object);
            EditorGUI.BeginDisabledGroup(!targetObjModifiable);
            {
                _newHideFlags = (HideFlags)EditorGUILayout.EnumFlagsField("New Hide Flags", _newHideFlags);

                EditorGUILayout.Space();
                if (GUILayout.Button("Set New Hide Flags"))
                {
                    Undo.RecordObject(_object, "Set HideFlags");
                    _object.hideFlags = _newHideFlags;

                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        public void SetTarget(UObject obj)
        {
            _object = obj;
            if (_inspector != null)
            {
                Remove(_inspector);
            }

            _inspector = new InspectorElement(_object)
            {
                name = "ObjectInspector",
                style =
                {
                    flexGrow = 1,
                },
            };
            Add(_inspector);
        }
    }
}
