using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;
using ItemChanger.Internal;
using ItemChanger.Locations;
using ItemChanger.Placements;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Hackish fix to make World Sense and Focus locations automatically compatible with the corresponding Lore Tablets.
    /// </summary>
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
            return !Ref.Settings.Placements.Values.OfType<AutoPlacement>()
                .Select(ap => ap.Location)
                .OfType<ExistingContainerLocation>()
                .Any(ecl => ecl.sceneName == sceneName && ecl.containerType == Container.Tablet
                && ecl.objectName == "Tut_tablet_top" && ecl.fsmName == "Inspection");
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
