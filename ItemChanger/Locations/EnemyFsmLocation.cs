using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    /// <summary>
    /// /A variant of EnemyLocation which accounts for the fact that some enemies may not be loaded at activeSceneChanged, and are easier to locate by fsm.
    /// </summary>
    public class EnemyFsmLocation : ContainerLocation
    {
        // enemy info - look for fsm in OnEnable, rather than object on scene entry
        public string enemyFsm;
        public string enemyObj;
        public bool removeGeo;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(enemyObj, enemyFsm), OnEnable);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(enemyObj, enemyFsm), OnEnable);
        }

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest || containerType == Container.Totem) return false;
            if (Container.GetContainer(containerType) is not Container c || !c.SupportsDrop) return false;
            return base.Supports(containerType);
        }

        public void OnEnable(PlayMakerFSM fsm)
        {
            GameObject enemy = fsm.gameObject;
            HealthManager hm = enemy.GetComponent<HealthManager>();
            hm.OnDeath += OnDeath;

            if (removeGeo)
            {
                hm.SetGeoSmall(0);
                hm.SetGeoMedium(0);
                hm.SetGeoLarge(0);
            }

            void OnDeath()
            {
                GiveEarly(enemy.transform);
                Placement.AddVisitFlag(VisitState.Dropped);
                GetContainer(out GameObject obj, out string containerType);
                Container c = Container.GetContainer(containerType);
                c.ApplyTargetContext(obj, enemy.transform.position.x, enemy.transform.position.y, 0);
            }
        }

        private void GiveEarly(Transform t)
        {
            Util.ItemUtility.GiveSequentially(
                Placement.Items.Where(i => i.GiveEarly("Enemy")), 
                Placement,
                new GiveInfo 
                {
                    Container = "Enemy",
                    FlingType = flingType,
                    MessageType = MessageType.Corner,
                    Transform = t,
                });
        }
    }
}
