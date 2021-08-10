using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
    public class DestroyObjectTag : Tag, IActiveSceneChangedTag
    {
        public override bool Intrinsic => true;

        public string objectName;
        public string sceneName;

        public void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name == sceneName)
            {
                GameObject obj = Locations.ObjectLocation.FindGameObject(objectName);
                if (obj) GameObject.Destroy(obj);
            }
        }
    }
}
