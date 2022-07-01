using ItemChanger.Components;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying lore tablets.
    /// </summary>
    public class TabletContainer : Container
    {
        public override string Name => Container.Tablet;
        public override bool SupportsInstantiate => true;

        public override GameObject GetNewContainer(ContainerInfo info)
        {
            return TabletUtility.MakeNewTablet(info);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            TabletUtility.ModifyTablet(fsm, info.flingType, info.placement, info.items);
        }

        public override void ApplyTargetContext(GameObject obj, float x, float y, float elevation)
        {
            obj.transform.position = new Vector3(x, y - elevation, 2.5f);
            obj.SetActive(true);
        }
    }
}
