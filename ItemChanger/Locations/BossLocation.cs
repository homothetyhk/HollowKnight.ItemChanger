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
    // A variant of EnemyLocation which accounts for the fact that bosses are not always loaded, and are usually not loaded at activeSceneChanged
    public class BossLocation : IMutableLocation
    {
        private GameObject obj;
        private Container container;

        // boss info - look for fsm in OnEnable, rather than object on scene entry
        public string bossFsm;
        public string bossObj;

        // fallback
        public string pdBool;
        public IMutableLocation fallbackLocation;

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

        public virtual void OnEnable(PlayMakerFSM fsm) 
        {
            if (fsm.FsmName == bossFsm && fsm.gameObject.name == bossObj && obj != null)
            {
                GameObject target = fsm.gameObject;
                HealthManager hm = target.GetComponent<HealthManager>();
                DropItemOnDeath drop = target.AddComponent<DropItemOnDeath>();
                drop.item = obj;
                drop.container = container;
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
        public virtual void OnActiveSceneChanged() 
        {
            fallbackLocation.OnActiveSceneChanged();
        }
        public virtual void Hook() 
        {
            fallbackLocation.Hook();
        }
        public virtual void UnHook() 
        {
            fallbackLocation.UnHook();
        }

        public virtual void PlaceContainer(GameObject obj, Container containerType)
        {
            if (PlayerData.instance.GetBool(pdBool))
            {
                fallbackLocation.PlaceContainer(obj, containerType);
                return;
            }

            this.obj = obj;
            this.container = containerType;
            obj.SetActive(false);
        }
    }
}
