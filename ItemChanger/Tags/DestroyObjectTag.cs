using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for destroying an object in a specific scene. Can search by name or by path.
    /// </summary>
    public class DestroyObjectTag : Tag
    {
        public string objectName;
        public string sceneName;

        public override void Load(object parent)
        {
            Events.AddSceneChangeEdit(sceneName, DestroyObject);
        }

        public override void Unload(object parent)
        {
            Events.RemoveSceneChangeEdit(sceneName, DestroyObject);
        }

        public void DestroyObject(Scene to)
        {
            if (to.name == sceneName)
            {
                GameObject obj = Locations.ObjectLocation.FindGameObject(objectName);
                if (obj)
                {
                    GameObject.Destroy(obj);
                    //ItemChangerMod.instance.Log($"Destroyed object {objectName} in {sceneName}");
                }
                //else ItemChangerMod.instance.Log($"Could not find object {objectName} in {sceneName}");
            }
        }
    }
}
