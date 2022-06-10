using ItemChanger.FsmStateActions;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which spawns a certain amount of soul, and requests a soul totem container.
    /// </summary>
    public class SoulItem : AbstractItem
    {
        /// <summary>
        /// The amount of soul to be given. This will be rounded up to the next even integer if given through spawned soul orbs.
        /// </summary>
        public int soul;

        public override bool GiveEarly(string containerType)
        {
            return containerType switch
            {
                Container.Enemy 
                or Container.Chest 
                or Container.GeoRock 
                or Container.GrubJar 
                or Container.Mimic
                or Container.Totem
                or Container.Bluggsac
                  => true,
                _ => false,
            };
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (HeroController.SilentInstance == null)
            {
                PlayerData.instance.AddMPCharge(soul);
                return;
            }

            if (info.FlingType == FlingType.DirectDeposit)
            {
                HeroController.SilentInstance.AddMPCharge(soul);
            }
            else if (info.Transform != null)
            {
                FlingSoulAction.SpawnSoul(info.Transform, soul, 11);
            }
            else
            {
                FlingSoulAction.SpawnSoul(HeroController.SilentInstance.transform, soul, 11);
            }
        }
    }
}
