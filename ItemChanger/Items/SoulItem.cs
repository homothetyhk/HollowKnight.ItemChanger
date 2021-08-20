using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Items
{
    public class SoulItem : AbstractItem
    {
        /// <summary>
        /// The amount of soul to be given. This will be rounded up to the next even integer if given through spawned soul orbs.
        /// </summary>
        public int soul;

        public override bool GiveEarly(string containerType)
        {
            switch (containerType)
            {
                case Container.Enemy:
                case Container.Chest:
                case Container.GeoRock:
                case Container.GrubJar:
                    return true;
                default:
                    return false;
            }
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.FlingType == FlingType.DirectDeposit)
            {
                HeroController.instance.AddMPCharge(soul);
            }
            else if (info.Transform != null)
            {
                RandomizerAddSoul.SpawnSoul(info.Transform, soul, 11);
            }
            else
            {
                RandomizerAddSoul.SpawnSoul(HeroController.instance.transform, soul, 11);
            }
        }
    }
}
