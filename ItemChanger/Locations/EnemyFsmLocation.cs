﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;
using ItemChanger.Placements;
using UnityEngine;

namespace ItemChanger.Locations
{
    // A variant of EnemyLocation which accounts for the fact that some enemies may not be loaded at activeSceneChanged
    public class EnemyFsmLocation : ContainerLocation
    {
        // enemy info - look for fsm in OnEnable, rather than object on scene entry
        public string enemyFsm;
        public string enemyObj;

        public bool removeGeo;

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest) return false;
            return base.Supports(containerType);
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == enemyFsm && fsm.gameObject.name == enemyObj)
            {
                base.GetContainer(out GameObject obj, out _);
                AddDeathEvent(fsm.gameObject, obj);
            }
        }

        public void AddDeathEvent(GameObject enemy, GameObject item)
        {
            HealthManager hm = enemy.GetComponent<HealthManager>();
            SpawnOnDeath drop = enemy.AddComponent<SpawnOnDeath>();
            drop.item = item;
            item.SetActive(false);

            if (removeGeo)
            {
                hm.SetGeoSmall(0);
                hm.SetGeoMedium(0);
                hm.SetGeoLarge(0);
            }
        }
    }
}
