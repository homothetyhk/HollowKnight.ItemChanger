using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class EnemyLocation : IMutableLocation
    {
        public string objectName;
        public string sceneName { get; set; }
        public FlingType flingType { get; set; }
        public bool forceShiny { get; set; }
        public bool removeGeo;

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
            GameObject target = ObjectLocation.FindGameObject(objectName);
            HealthManager hm = target.GetComponent<HealthManager>();

            DropItemOnDeath drop = target.AddComponent<DropItemOnDeath>();
            drop.item = obj;
            drop.container = containerType;
            drop.flingType = flingType;
            obj.SetActive(false);   

            if (removeGeo)
            {
                hm.SetGeoSmall(0);
                hm.SetGeoMedium(0);
                hm.SetGeoLarge(0);
            }
        }
    }
}
