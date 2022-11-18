using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;
using ItemChanger.Util;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ContainerLocation for dropping an item when Gruz Mother is killed.
    /// </summary>
    public class GruzMotherDropLocation : ContainerLocation
    {
        public bool removeGeo = true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("burster"), EditCorpseBurst);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("burster"), EditCorpseBurst);
        }

        private void EditCorpseBurst(PlayMakerFSM fsm)
        {
            if (!fsm.gameObject.name.StartsWith("Corpse Big Fly Burster")) return;

            FsmState geo = fsm.GetState("Geo");
            if (removeGeo) geo.ClearActions();
            geo.AddLastAction(new Lambda(() => PlaceContainer(fsm.gameObject)));
        }

        private void PlaceContainer(GameObject gruz)
        {
            base.GetContainer(out GameObject obj, out string containerType);
            Container.GetContainer(containerType).ApplyTargetContext(obj, gruz, 0);
            if (containerType == Container.Shiny && !Placement.GetPlacementAndLocationTags().OfType<Tags.ShinyFlingTag>().Any())
            {
                ShinyUtility.SetShinyFling(obj.LocateMyFSM("Shiny Control"), ShinyFling.RandomLR);
            }
        }
    }
}
