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
        public override bool SupportsInstantiate => Internal.ObjectCache.GrubPreloader.PreloadLevel != Internal.PreloadLevel.None;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null)
        {
            return GrubJarUtility.MakeNewGrubJar(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Bottle Control") return;
            GrubJarUtility.ModifyBottleFsm(fsm, info.flingType, info.placement, info.items);
        }

        public override void ApplyTargetContext(GameObject obj, GameObject target, float elevation)
        {
            GrubJarUtility.MoveGrubJar(obj, target, elevation);
        }

        public override void ApplyTargetContext(GameObject obj, float x, float y, float elevation)
        {
            GrubJarUtility.MoveGrubJar(obj, x, y, elevation);
        }
    }
}
