using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HierarchyTreeViewElement : IMGUIContainer
    {
        public HierarchyTreeView HierarchyTreeView { get; }
        public TreeViewItem RootItem => HierarchyTreeView.Root;


        public HierarchyTreeViewElement(TreeViewState treeViewState)
        {
            onGUIHandler = OnGUI;
            HierarchyTreeView = new HierarchyTreeView(treeViewState);
        }

        private void OnGUI()
        {
            Rect drawArea = localBound;

            var c = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            GUI.Box(drawArea, "test");
            GUI.backgroundColor = c;

            HierarchyTreeView.OnGUI(drawArea);
        }

        public int GetUniqueItemId() => HierarchyTreeView.GetUniqueItemId();
        public void ClearItems() => HierarchyTreeView.ClearItems();
        public void Reload() => HierarchyTreeView.Reload();
        public void ExpandAll() => HierarchyTreeView.ExpandAll();
        public void CollapseAll() => HierarchyTreeView.CollapseAll();
    }
}
