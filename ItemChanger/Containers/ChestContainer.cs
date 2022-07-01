using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying chests.
    /// </summary>
    public class ChestContainer : Container
    {
        public override string Name => Container.Chest;
        public override bool SupportsDrop => true;
        public override bool SupportsInstantiate => true;

        public override GameObject GetNewContainer(ContainerInfo info)
        {
            return ChestUtility.MakeNewChest(info);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Chest Control") return;
            ChestUtility.ModifyChest(fsm, info.flingType, info.placement, info.items);
            base.AddGiveEffectToFsm(fsm, info);
        }

        public override void ApplyTargetContext(GameObject obj, float x, float y, float elevation)
        {
            ChestUtility.MoveChest(obj, x, y, elevation);
        }

        public override void ApplyTargetContext(GameObject obj, GameObject target, float elevation)
        {
            ChestUtility.MoveChest(obj, target, elevation);
        }
    }
}
