using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Placements;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Base type for finding and replacing a game object with an item container
    /// </summary>
    public class ObjectLocation : ContainerLocation
    {
        public string objectName;
        public float elevation;

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            base.OnActiveSceneChanged(from, to);
            if (to.name == sceneName)
            {
                base.GetContainer(out GameObject obj, out string containerType);
                PlaceContainer(obj, containerType);
            }
        }

        public virtual void PlaceContainer(GameObject obj, string containerType)
        {
            GameObject target = FindGameObject(objectName);
            if (!target)
            {
                ItemChanger.instance.LogError($"Unable to find {objectName} for ObjectLocation {name}!");
                return;
            }

            Container.GetContainer(containerType).ApplyTargetContext(obj, target, elevation);
            GameObject.Destroy(target);
        }


        public static GameObject FindGameObject(string objectName)
        {
            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            string[] objectHierarchy = objectName.Split('\\');
            int i = 1;
            GameObject obj = currentScene.FindGameObject(objectHierarchy[0]);
            while (i < objectHierarchy.Length)
            {
                obj = obj.FindGameObjectInChildren(objectHierarchy[i++]);
            }

            return obj;
        }
    }
}
