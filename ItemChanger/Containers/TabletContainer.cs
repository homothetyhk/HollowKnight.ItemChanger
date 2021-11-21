using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying lore tablets.
    /// </summary>
    public class TabletContainer : Container
    {
        public override string Name => Container.Tablet;
        public override bool SupportsInstantiate => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null, Transition? changeSceneTo = null)
        {
            return TabletUtility.MakeNewTablet(placement, items, flingType);
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
