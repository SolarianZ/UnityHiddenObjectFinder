using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PackagesGBG.HiddenObjectFinder.Editor
{
    public class HierarchyTreeView : TreeView
    {
        public TreeViewItem Root { get; }

        private int _nextUniqueItemId = 1;


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
    }
}
