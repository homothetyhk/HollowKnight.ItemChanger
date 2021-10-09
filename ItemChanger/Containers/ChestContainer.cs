using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    public class ChestContainer : Container
    {
        public override string Name => Container.Chest;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null)
        {
            return ChestUtility.MakeNewChest(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Chest Control") return;
            ChestUtility.ModifyChest(fsm, info.flingType, info.placement, info.items);
            base.AddGiveEffectToFsm(fsm, info);
        }
    }
}
