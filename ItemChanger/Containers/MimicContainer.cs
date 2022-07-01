using ItemChanger.Components;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying mimics.
    /// </summary>
    public class MimicContainer : Container
    {
        public override string Name => Mimic;
        public override bool SupportsDrop => true;
        public override bool SupportsInstantiate => Internal.ObjectCache.MimicPreloader.PreloadLevel != Internal.PreloadLevel.None;

        public override GameObject GetNewContainer(ContainerInfo info)
        {
            return MimicUtil.CreateNewMimic(info);
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
