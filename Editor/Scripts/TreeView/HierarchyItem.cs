using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HierarchyItem
    {
        public UObject Object { get; }
        public UObject Node => Object is Component comp ? comp.gameObject : Object;
        public HierarchyItem Parent { get; set; }
        public bool IsRoot => Parent == null;
        public List<HierarchyItem> Children { get; } = new List<HierarchyItem>();

        public HierarchyItem(UObject obj)
        {
            Object = obj;
        }

        public string GetDisplayName()
        {
            if (!Object)
            {
                return "Missing Object";
            }

            if (Object is Component comp)
            {
                return $"[{comp.GetType().Name}]";
            }

            return Object.name;
        }

        public static List<HierarchyItem> BuildHierarchy(IList<UObject> objects)
        {
            Dictionary<UObject, HierarchyItem> registry = new Dictionary<UObject, HierarchyItem>();
            foreach (UObject obj in objects)
            {
                BuildHierarchyInternal(obj, registry);
            }

            List<HierarchyItem> rootItems = registry.Values.Where(item => item.IsRoot).ToList();
            rootItems.Sort((a, b) =>
            {
                int aIndex = objects.IndexOf(a.Object);
                int bIndex = objects.IndexOf(b.Object);
                if (aIndex < bIndex)
                    return -1;
                if (aIndex > bIndex)
                    return 1;
                return 0;
            });

            return rootItems;
        }

        private static HierarchyItem BuildHierarchyInternal(UObject obj, Dictionary<UObject, HierarchyItem> registry)
        {
            Assert.IsNotNull(obj);
            Assert.IsNotNull(registry);

            if (registry.TryGetValue(obj, out HierarchyItem item))
            {
                return item;
            }

            item = new HierarchyItem(obj);
            registry.Add(obj, item);

            if (obj is Component comp)
            {
                GameObject compGo = comp.gameObject;
                if (!registry.TryGetValue(compGo, out HierarchyItem compGoItem))
                {
                    compGoItem = new HierarchyItem(compGo);
                    registry.Add(compGo, compGoItem);
                }

                compGoItem.Children.Add(item);
                item.Parent = compGoItem;

                Transform parent = compGo.transform.parent;
                if (parent)
                {
                    HierarchyItem parentItem = BuildHierarchyInternal(parent.gameObject, registry);
                    parentItem.Children.Add(compGoItem);
                    compGoItem.Parent = parentItem;
                }

                return item;
            }

            if (obj is GameObject go)
            {
                Transform goParent = go.transform.parent;
                if (goParent)
                {
                    HierarchyItem parentItem = BuildHierarchyInternal(goParent.gameObject, registry);
                    parentItem.Children.Add(item);
                    item.Parent = parentItem;
                }
            }

            return item;
        }
    }
}