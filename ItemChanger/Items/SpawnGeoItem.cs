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

        public override bool GiveEarly(string containerType)
        {
            return containerType switch
            {
                Container.Enemy 
                or Container.Chest 
                or Container.GeoRock 
                or Container.GrubJar 
                or Container.Mimic
                  => true,
                _ => false,
            };
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.FlingType == FlingType.DirectDeposit || info.Transform == null)
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
            FsmStateActions.RandomizerAddGeo.SpawnGeo(amount, false, info.FlingType, info.Transform);
        }
    }
}
