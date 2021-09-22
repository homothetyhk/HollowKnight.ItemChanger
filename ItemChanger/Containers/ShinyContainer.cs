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

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return ShinyUtility.MakeNewMultiItemShiny(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            ShinyUtility.ModifyMultiShiny(fsm, info.flingType, info.placement, info.items);
            base.AddGiveEffectToFsm(fsm, info);
        }

        public override void AddChangeSceneToFsm(PlayMakerFSM fsm, ChangeSceneInfo info)
        {
            ShinyUtility.AddChangeSceneToShiny(fsm, info.toScene, info.toGate);
            base.AddChangeSceneToFsm(fsm, info);
        }

        public override void AddCostToFsm(PlayMakerFSM fsm, CostInfo info)
        {
            ShinyUtility.AddYNDialogueToShiny(fsm, info.cost, info.previewItems);
            base.AddCostToFsm(fsm, info);
        }
    }
}
