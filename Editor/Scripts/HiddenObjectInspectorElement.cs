using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectInspectorElement : VisualElement
    {
        private VisualElement _inspector;
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
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Set New Hide Flags"))
                    {
                        Undo.RecordObject(_object, "Set HideFlags");
                        _object.hideFlags = _newHideFlags;
                        EditorUtility.SetDirty(_object);
                        EditorApplication.RepaintHierarchyWindow();
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(100)))
                    {
                        TryDeleteObject();
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }

        public void SetTarget(UObject obj)
        {
            _object = obj;
            if (_inspector != null && Children().Contains(_inspector))
            {
                Remove(_inspector);
            }

            if (_object)
            {
                _inspector = new InspectorElement(_object)
                {
                    name = "ObjectInspector",
                    style =
                    {
                        flexGrow = 1,
                    },
                };
            }
            else
            {
                _inspector = new Label("The object has been deleted.");
            }

            Add(_inspector);
        }


        private void TryDeleteObject()
        {
            if (_object is Component comp)
            {
                if (comp is Transform transform)
                {
                    EditorUtility.DisplayDialog("Hidden Object Finder - Error",
                        "The Transfrom component cannot be removed.", "OK");
                    return;
                }

                if (IsRequiredComponent(comp, out Component requiredBy))
                {
                    bool remove = EditorUtility.DisplayDialog("Hidden Object Finder - Warning",
                        $"The component \"{comp.GetType().Name}\" you want to remove is being required by component \"{requiredBy.GetType().Name}\". Do you still want to remove it?",
                        "Remove", "Cancel");
                    if (!remove)
                    {
                        return;
                    }
                }
            }

            Undo.DestroyObjectImmediate(_object);
            EditorApplication.RepaintHierarchyWindow();
        }

        private static bool IsRequiredComponent(Component component, out Component requiredBy)
        {
            if (!component)
            {
                requiredBy = null;
                return false;
            }

            Type compType = component.GetType();
            Component[] allComponents = component.gameObject.GetComponents<Component>();
            foreach (Component comp in allComponents)
            {
                if (comp == component)
                {
                    continue;
                }

                IEnumerable<RequireComponent> requireCompAttrs = comp.GetType().GetCustomAttributes<RequireComponent>();
                foreach (RequireComponent requireCompAttr in requireCompAttrs)
                {

                    if ((requireCompAttr.m_Type0?.IsAssignableFrom(compType) ?? false) ||
                        (requireCompAttr.m_Type1?.IsAssignableFrom(compType) ?? false) ||
                        (requireCompAttr.m_Type2?.IsAssignableFrom(compType) ?? false))
                    {
                        requiredBy = comp;
                        return true;
                    }
                }
            }

            requiredBy = null;
            return false;
        }
    }
}