using ItemChanger.Util;

namespace ItemChanger.Locations
{
    /// <summary>
    /// A location for modifying an enemy to drop an item container on death.
    /// </summary>
    public class EnemyLocation : ContainerLocation
    {
        public string objectName;
        public bool removeGeo;

        protected override void OnLoad()
        {
            Events.AddSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        protected override void OnUnload()
        {
            Events.RemoveSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest || containerType == Container.Totem) return false;
            if (Container.GetContainer(containerType) is not Container c || !c.SupportsDrop) return false;
            return base.Supports(containerType);
        }

        public void OnActiveSceneChanged(Scene to)
        {
            GameObject enemy = ObjectLocation.FindGameObject(objectName);
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
            ItemUtility.GiveSequentially(
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
