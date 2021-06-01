using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    // TODO: Can this be done outside of the fsm?
    public class SpawnGeoItem : AbstractItem
    {
        public int amount;

        public override bool GiveEarly(Container container)
        {
            switch (container)
            {
                case Container.Chest:
                case Container.GeoRock:
                case Container.GrubJar:
                    return true;
                default:
                    return false;
            }
        }

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            switch (fling)
            {
                case FlingType.DirectDeposit:
                    HeroController.instance.AddGeo(amount);
                    break;
                default:
                    FsmStateActions.RandomizerAddGeo.SpawnGeo(amount, false, fling, transform);
                    break;
            }
        }

    }
}
