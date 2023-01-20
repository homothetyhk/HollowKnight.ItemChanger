using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which replaces a lore tablet and must disable its inspect region.
    /// </summary>
    public class LoreTabletLocation : ObjectLocation
    {
        public string? inspectName;
        public string inspectFsm;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new(inspectName, inspectFsm), DisableInspectRegion);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new(inspectName, inspectFsm), DisableInspectRegion);
        }

        private void DisableInspectRegion(PlayMakerFSM fsm)
        {
            if (fsm.GetState("Init") is FsmState init)
            {
                init.ClearTransitions();
            }

            if (fsm.GetState("Inert") is FsmState inert)
            {
                inert.ClearTransitions();
            }
        }
    }
}
