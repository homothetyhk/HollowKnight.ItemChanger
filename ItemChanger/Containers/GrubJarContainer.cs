using ItemChanger.Components;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying grubs.
    /// </summary>
    public class GrubJarContainer : Container
    {
        public override string Name => Container.GrubJar;
        public override bool SupportsInstantiate => Internal.ObjectCache.GrubPreloader.PreloadLevel != Internal.PreloadLevel.None;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(ContainerInfo info)
        {
            return GrubJarUtility.MakeNewGrubJar(info);
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
