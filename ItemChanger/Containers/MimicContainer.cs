using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Components;
using ItemChanger.Util;
using UnityEngine;

namespace ItemChanger.Containers
{
    public class MimicContainer : Container
    {
        public override string Name => Mimic;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return MimicUtil.CreateNewMimic(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName == "Grub Control")
            {
                MimicUtil.ModifyMimic(fsm, info.flingType, info.placement, info.items);
            }
        }
    }
}
