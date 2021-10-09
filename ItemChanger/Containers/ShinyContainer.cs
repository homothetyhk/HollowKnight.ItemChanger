using ItemChanger.Components;
using ItemChanger.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    public class ShinyContainer : Container
    {
        public override string Name => Container.Shiny;
        public override bool SupportsCost => true;
        public override bool SupportsSceneChange => true;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null)
        {
            return ShinyUtility.MakeNewMultiItemShiny(placement, items, flingType, cost);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Shiny Control") return;
            ShinyUtility.ModifyMultiShiny(fsm, info.flingType, info.placement, info.items);
        }

        public override void AddChangeSceneToFsm(PlayMakerFSM fsm, ChangeSceneInfo info)
        {
            if (fsm.FsmName != "Shiny Control") return;
            ShinyUtility.AddChangeSceneToShiny(fsm, info.toScene, info.toGate);
        }

        public override void AddCostToFsm(PlayMakerFSM fsm, CostInfo info)
        {
            if (fsm.FsmName != "Shiny Control") return;
            ShinyUtility.AddYNDialogueToShiny(fsm, info.cost, info.previewItems);
        }
    }
}
