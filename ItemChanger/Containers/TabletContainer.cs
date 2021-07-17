using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    public class TabletContainer : Container
    {
        public override string Name => Container.Tablet;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return TabletUtility.MakeNewTablet(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            TabletUtility.ModifyTablet(fsm, info.flingType, info.placement, info.items);
            base.AddGiveEffectToFsm(fsm, info);
        }
    }
}
