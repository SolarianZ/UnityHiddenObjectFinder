using UnityEditor;
using UnityEngine;

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


        #region Unity Messages

        private void ShowButton(Rect position)
        {
            if (GUI.Button(position, EditorGUIUtility.IconContent("_Help"), GUI.skin.FindStyle("IconButton")))
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityHiddenObjectFinder");
            }
        }

        private void OnGUI()
        {

        }

        #endregion


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
    }
}
