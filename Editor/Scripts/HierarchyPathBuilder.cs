using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.HiddenObjectFinder.Editor
{
    public static class HierarchyPathBuilder
    {
        public static string BuildHierarchyPath(this Component component)
        {
            if (!component)
            {
                throw new ArgumentNullException(nameof(component));
            }

            Transform transform = component.transform;
            StringBuilder builder = new StringBuilder();
            builder.Append(transform.name);

            Transform parent = transform.parent;
            while (parent)
            {
                builder.Insert(0, '/').Insert(0, parent.name);
                parent = parent.parent;
            }

            return builder.ToString();
        }

        public static string BuildHierarchyPath(this GameObject go)
        {
            if (!go)
            {
                throw new ArgumentNullException(nameof(go));
            }

            return go.transform.BuildHierarchyPath();
        }

        public static List<string> BuildHierarchyPathParts(this Component component)
        {
            if (!component)
            {
                throw new ArgumentNullException(nameof(component));
            }

            List<string> pathParts = new List<string>();
            component.BuildHierarchyPathInternal(pathParts);

            return pathParts;
        }

        public static List<string> BuildHierarchyPathParts(this GameObject go)
        {
            if (!go)
            {
                throw new ArgumentNullException(nameof(go));
            }

            return go.transform.BuildHierarchyPathParts();
        }

        private static void BuildHierarchyPathInternal(this Component component, List<string> pathParts)
        {
            Assert.IsNotNull(component);
            Assert.IsNotNull(pathParts);

            Transform transform = component.transform;
            while (transform)
            {
                pathParts.Insert(0, transform.name);
                transform = transform.parent;
            }
        }
    }
}