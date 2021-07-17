using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    public class GeoRockContainer : Container
    {
        public override string Name => Container.GeoRock;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return GeoRockUtility.MakeNewGeoRock(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Geo Rock") return;
            GeoRockUtility.ModifyGeoRock(fsm, info.flingType, info.placement, info.items);
            base.AddGiveEffectToFsm(fsm, info);
        }

    }
}
