using UnityEditor.IMGUI.Controls;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HierarchyTreeViewItem : TreeViewItem
    {
        public HierarchyItem HierarchyItem { get; }

        public HierarchyTreeViewItem(int id, int depth, HierarchyItem hierarchyItem)
            : base(id, depth, hierarchyItem.GetDisplayName())
        {
            HierarchyItem = hierarchyItem;
        }
    }
}