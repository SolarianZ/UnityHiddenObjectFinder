using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public static class HiddenObjectFinder
    {
        public static List<GameObject> FindGameObjects(HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            return FindAll<GameObject>(hideFlagsFilter);
        }

        public static List<GameObject> FindGameObjects(this Scene scene, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (scene.isLoaded)
            {
                return new List<GameObject>(0);
            }

            List<GameObject> hiddenGoList = new List<GameObject>();
            foreach (GameObject rootGo in scene.GetRootGameObjects())
            {
                rootGo.FindHiddenGameObjectInHierarchy(hiddenGoList, hideFlagsFilter);
            }

            return hiddenGoList;
        }

        public static void FindHiddenGameObjectInHierarchy(this Transform rootGo, List<GameObject> resultForAppend, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (resultForAppend == null)
            {
                throw new ArgumentNullException(nameof(resultForAppend));
            }

            if (!rootGo)
            {
                return;
            }

            rootGo.gameObject.FindHiddenGameObjectInHierarchy(resultForAppend, hideFlagsFilter);
        }

        public static void FindHiddenGameObjectInHierarchy(this GameObject rootGo, List<GameObject> resultForAppend, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (resultForAppend == null)
            {
                throw new ArgumentNullException(nameof(resultForAppend));
            }

            if (!rootGo)
            {
                return;
            }

            if (rootGo.MatchHideFlags(hideFlagsFilter))
            {
                resultForAppend.Add(rootGo);
            }

            Transform rootTransform = rootGo.transform;
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                GameObject childGo = rootTransform.GetChild(i).gameObject;
                childGo.FindHiddenGameObjectInHierarchy(resultForAppend, hideFlagsFilter);
            }
        }

        public static List<UObject> FindAll(bool excludeComponents, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            List<UObject> allHiddenObjects = FindAll<UObject>(hideFlagsFilter);
            List<UObject> nonCompHiddenObjects = allHiddenObjects.Where(obj => !(obj is Component)).ToList();
            return nonCompHiddenObjects;
        }

        public static List<T> FindAll<T>(HideFlags hideFlagsFilter = HideFlags.HideInHierarchy) where T : UObject
        {
            T[] allObjects = UObject.FindObjectsOfType<T>();
            List<T> hiddenObjects = allObjects.Where(obj => obj.MatchHideFlags(hideFlagsFilter))
                .ToList();
            return hiddenObjects;
        }


        public static bool MatchHideFlags(this UObject obj, HideFlags hideFlagsFilter)
        {
            return obj && (obj.hideFlags & hideFlagsFilter) != 0;
        }
    }
}