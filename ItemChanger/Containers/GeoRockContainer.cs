using ItemChanger.Util;
using ItemChanger.Components;

namespace ItemChanger.Containers
{
    /// <summary>
    /// Container for creating and modifying geo rocks.
    /// </summary>
    public class GeoRockContainer : Container
    {
        public override string Name => Container.GeoRock;
        public override bool SupportsDrop => true;
        public override bool SupportsInstantiate => Internal.ObjectCache.GeoRockPreloader.PreloadLevel != Internal.PreloadLevel.None;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null, Transition? changeSceneTo = null)
        {
            return GeoRockUtility.MakeNewGeoRock(placement, items, flingType);
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Geo Rock") return;
            GeoRockUtility.ModifyGeoRock(fsm, info.flingType, info.placement, info.items);
        }

        public override void ApplyTargetContext(GameObject obj, float x, float y, float elevation)
        {
            GeoRockUtility.SetRockContext(obj, x, y, elevation);
        }

        public override void ApplyTargetContext(GameObject obj, GameObject target, float elevation)
        {
            GeoRockUtility.SetRockContext(obj, target, elevation);
        }
    }
}
