using ItemChanger.Extensions;
using ItemChanger.Internal;
using ItemChanger.Placements;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Hackish fix to make World Sense and Focus locations automatically compatible with the corresponding Lore Tablets.
    /// </summary>
    [DefaultModule]
    public class LocationCompatibility : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Tutorial_01, new("Tut_tablet_top", "Inspection"), HideFocusInspect);
            Events.AddFsmEdit(SceneNames.Room_Final_Boss_Atrium, new("Tut_tablet_top", "Inspection"), HideWorldSenseInspect);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Tutorial_01, new("Tut_tablet_top", "Inspection"), HideFocusInspect);
            Events.RemoveFsmEdit(SceneNames.Room_Final_Boss_Atrium, new("Tut_tablet_top", "Inspection"), HideWorldSenseInspect);
        }

        private bool ShouldHideTabletInspect(string placementName, string sceneName)
        {
            // if placement is not modified, do nothing
            if (!Ref.Settings.Placements.ContainsKey(placementName)) return false;

            // if placement is modified, then hide the tablet inspect if no ECL uses it
            return !Ref.Settings.Placements.Values.OfType<ExistingContainerPlacement>()
                .Select(ap => ap.Location)
                .Any(ecl => ecl.sceneName == sceneName && ecl.containerType == Container.Tablet && ecl is Locations.ExistingFsmContainerLocation efcl
                && efcl.objectName == "Tut_tablet_top" && efcl.fsmName == "Inspection");
        }

        private void HideFocusInspect(PlayMakerFSM fsm)
        {
            if (ShouldHideTabletInspect("Focus", SceneNames.Tutorial_01))
            {
                fsm.GetState("Init").ClearTransitions();
            }
        }

        private void HideWorldSenseInspect(PlayMakerFSM fsm)
        {
            if (ShouldHideTabletInspect("World_Sense", SceneNames.Room_Final_Boss_Atrium))
            {
                fsm.GetState("Init").ClearTransitions();
            }
        }
    }
}
