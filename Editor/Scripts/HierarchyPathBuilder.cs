using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public static class HierarchyPathBuilder
    {
        public static string BuildHierarchyPath(this UObject obj)
        {
            if (!obj)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is GameObject go)
            {
                return go.BuildHierarchyPath();
            }
            else if (obj is Component comp)
            {
                return comp.BuildHierarchyPath();
            }
            else
            {
                return obj.name;
            }
        }

        public static string BuildHierarchyPath(this GameObject go)
        {
            if (!go)
            {
                throw new ArgumentNullException(nameof(go));
            }

            return go.transform.BuildHierarchyPath();
        }

        public static string BuildHierarchyPath(this Component component)
        {
            if (!component)
            {
                throw new ArgumentNullException(nameof(component));
            }

            StringBuilder builder = new StringBuilder();
            builder.Append($"::{component.GetType().Name}");

            Transform transform = component.transform;
            while (transform)
            {
                builder.Insert(0, '/').Insert(0, transform.name);
                transform = transform.parent;
            }

            return builder.ToString();
        }


        public static List<string> BuildHierarchyPathParts(this UObject obj)
        {
            if (!obj)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is GameObject go)
            {
                return go.BuildHierarchyPathParts();
            }
            else if (obj is Component comp)
            {
                return comp.BuildHierarchyPathParts();
            }
            else
            {
                return new List<string> { obj.name };
            }
        }

        public static List<string> BuildHierarchyPathParts(this GameObject go)
        {
            if (!go)
            {
                throw new ArgumentNullException(nameof(go));
            }

            List<string> pathParts = new List<string>();
            go.BuildHierarchyPathInternal(pathParts);

            return pathParts;
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

        private static void BuildHierarchyPathInternal(this GameObject go, List<string> pathParts)
        {
            Assert.IsNotNull(go);
            Assert.IsNotNull(pathParts);

            Transform transform = go.transform;
            while (transform)
            {
                pathParts.Insert(0, transform.name);
                transform = transform.parent;
            }
        }

        private static void BuildHierarchyPathInternal(this Component component, List<string> pathParts)
        {
            Assert.IsNotNull(component);
            Assert.IsNotNull(pathParts);

            pathParts.Add($"::{component.GetType().Name}");

            Transform transform = component.transform;
            while (transform)
            {
                pathParts.Insert(0, transform.name);
                transform = transform.parent;
            }
        }
    }
}