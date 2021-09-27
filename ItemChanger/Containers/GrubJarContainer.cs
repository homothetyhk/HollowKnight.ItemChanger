using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    public class GrubJarContainer : Container
    {
        public override string Name => Container.GrubJar;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return GrubJarUtility.MakeNewGrubJar(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Bottle Control") return;
            GrubJarUtility.ModifyBottleFsm(fsm, info.flingType, info.placement, info.items);
        }
    }
}
