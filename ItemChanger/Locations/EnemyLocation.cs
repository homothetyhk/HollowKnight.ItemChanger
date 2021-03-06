﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;
using ItemChanger.Placements;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class EnemyLocation : PlaceableLocation
    {
        public string objectName;
        public bool removeGeo;

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest) return false;
            return base.Supports(containerType);
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            base.OnActiveSceneChanged(from, to);
            if (!managed && to.name == sceneName)
            {
                base.GetContainer(out GameObject obj, out string containerType);
                PlaceContainer(obj, containerType);
            }
        }

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            GameObject target = ObjectLocation.FindGameObject(objectName);
            HealthManager hm = target.GetComponent<HealthManager>();

            SpawnOnDeath drop = target.AddComponent<SpawnOnDeath>();
            drop.item = obj;
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
