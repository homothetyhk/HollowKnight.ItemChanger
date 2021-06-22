using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Base type for finding and replacing a game object with an item container
    /// </summary>
    public class ObjectLocation : IMutableLocation
    {
        public string objectName;
        public float elevation;
        public string sceneName { get; set; }
        public FlingType flingType { get; set; }
        public bool forceShiny { get; set; }

        public bool Supports(Container container)
        {
            switch (container)
            {
                case Container.Chest:
                case Container.GeoRock:
                case Container.GrubJar:
                    return !forceShiny;
                case Container.Shiny:
                    return true;
                default:
                    return false;
            }
        }

        public virtual void OnEnable(PlayMakerFSM fsm) { }
        public virtual void OnActiveSceneChanged() { }
        public virtual void Hook() { }
        public virtual void UnHook() { }

        public virtual void PlaceContainer(GameObject obj, Container containerType)
        {
            GameObject target = FindGameObject(objectName);
            ContainerUtility.ApplyTargetContext(target, obj, containerType, elevation);
            GameObject.Destroy(target);
        }


        public static GameObject FindGameObject(string objectName)
        {
            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameManager.instance.StartCoroutine(Finder());
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (s, e) => ItemChanger.instance.Log(s.name);

            string[] objectHierarchy = objectName.Split('\\');
            int i = 1;
            GameObject obj = currentScene.FindGameObject(objectHierarchy[0]);
            while (i < objectHierarchy.Length)
            {
                obj = obj.FindGameObjectInChildren(objectHierarchy[i++]);
            }

            return obj;
        }

        public static IEnumerator Finder()
        {
            yield return null;
            yield return new WaitForFinishedEnteringScene();
            yield return new WaitForEndOfFrame();
            
            for (int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; j++)
            {
                ItemChanger.instance.Log(j);
                ItemChanger.instance.Log(UnityEngine.SceneManagement.SceneManager.GetSceneAt(j).name);
            }
            ItemChanger.instance.Log(UnityEngine.SceneManagement.SceneManager.sceneCount);
        }

    }
}
