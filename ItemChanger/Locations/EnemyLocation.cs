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
        private Action _cleanupAction;

        protected override void OnLoad()
        {
            Events.AddSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        protected override void OnUnload()
        {
            Events.RemoveSceneChangeEdit(sceneName, OnActiveSceneChanged);
            DoCleanup();
        }

        public override bool Supports(string containerType)
        {
            if (containerType == Container.Chest || containerType == Container.Totem) return false;
            if (Container.GetContainer(containerType) is not Container c || !c.SupportsDrop) return false;
            return base.Supports(containerType);
        }

        public void OnActiveSceneChanged(Scene to)
        {
            DoCleanup();

            GameObject enemy = ObjectLocation.FindGameObject(objectName);
            HealthManager hm = enemy.GetComponent<HealthManager>();
            if (!hm.hasSpecialDeath)
            {
                hm.OnDeath += OnDeath;
            }
            else
            {
                On.HealthManager.Die += OnSpecialDeath;
                _cleanupAction = () =>
                {
                    On.HealthManager.Die -= OnSpecialDeath;
                };
            }

            if (removeGeo)
            {
                hm.SetGeoSmall(0);
                hm.SetGeoMedium(0);
                hm.SetGeoLarge(0);
            }

            void OnSpecialDeath(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
            {
                if (self != hm || hm.isDead)
                {
                    orig(self, attackDirection, attackType, ignoreEvasion);
                    return;
                }
                else
                {
                    orig(self, attackDirection, attackType, ignoreEvasion);
                    OnDeath();
                }
            }

            void OnDeath()
            {
                GiveEarly(enemy.transform);
                Placement.AddVisitFlag(VisitState.Dropped);
                GetContainer(out GameObject obj, out string containerType);
                Container c = Container.GetContainer(containerType);
                c.ApplyTargetContext(obj, enemy.transform.position.x, enemy.transform.position.y, 0);
                if (containerType == Container.Shiny && !Placement.GetPlacementAndLocationTags().OfType<Tags.ShinyFlingTag>().Any())
                {
                    ShinyUtility.SetShinyFling(obj.LocateMyFSM("Shiny Control"), ShinyFling.RandomLR);
                }
                DoCleanup();
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

        private void DoCleanup()
        {
            if (_cleanupAction != null)
            {
                try { _cleanupAction.Invoke(); } catch { }
                _cleanupAction = null;
            }
        }
    }
}
