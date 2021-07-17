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

        public void OnActiveSceneChanged(Scene from, Scene to, AbstractPlacement placement)
        {
            if (to.name == placement.SceneName)
            {
                GameObject obj = Locations.ObjectLocation.FindGameObject(objectName);
                if (obj) GameObject.Destroy(obj);
            }
        }
    }
}
