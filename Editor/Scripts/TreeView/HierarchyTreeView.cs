using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HierarchyTreeView : TreeView
    {
        public TreeViewItem Root { get; }

        private int _nextUniqueItemId = 1;

        public event Action<HierarchyTreeViewItem> OnClickItem;


        public HierarchyTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Root = new TreeViewItem(0, -1)
            {
                children = new List<TreeViewItem>()
            };

            Reload();
        }

        public void ClearItems()
        {
            Root.children.Clear();
        }

        public int GetUniqueItemId()
        {
            int uniqueItemId = _nextUniqueItemId;
            _nextUniqueItemId++;
            return uniqueItemId;
        }

        protected override TreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(Root);
            return Root;
        }

        protected override void SingleClickedItem(int id)
        {
            //base.SingleClickedItem(id);
            HierarchyTreeViewItem clickedItem = FindItem(id, Root) as HierarchyTreeViewItem;
            if (clickedItem != null)
            {
                OnClickItem?.Invoke(clickedItem);
            }
        }
    }
}
