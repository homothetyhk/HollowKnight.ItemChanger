using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
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
            if (fling == FlingType.DirectDeposit || transform == null)
            {
                if (HeroController.instance != null)
                {
                    HeroController.instance.AddGeo(amount);
                }
                else
                {
                    PlayerData.instance.AddGeo(amount);
                }
                return;
            }
            FsmStateActions.RandomizerAddGeo.SpawnGeo(amount, false, fling, transform);
        }
    }
}
