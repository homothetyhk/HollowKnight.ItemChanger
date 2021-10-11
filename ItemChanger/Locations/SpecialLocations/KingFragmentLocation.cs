using ItemChanger.Components;
using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which supports a hint at the Kingsmould corpse in Abyss_05 and triggers a scene change to Abyss_05 when its items are obtained.
    /// </summary>
    public class KingFragmentLocation : ObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddLanguageEdit(new("Lore Tablets", "DUSK_KNIGHT_CORPSE"), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveLanguageEdit(new("Lore Tablets", "DUSK_KNIGHT_CORPSE"), OnLanguageGet);
        }

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            obj.GetOrAddComponent<ContainerInfo>().changeSceneInfo
                = new ChangeSceneInfo { toScene = "Abyss_05" };
            base.PlaceContainer(obj, containerType);
        }

        private void OnLanguageGet(ref string value)
        {
            if (HintActive)
            {
                string item = Placement.GetUIName();
                if (!string.IsNullOrEmpty(item))
                {
                    value = "A corpse in white armour. You can clearly see the "
                                + Placement.GetUIName() + " it's holding, " +
                                "but for some reason you get the feeling you're going to have to go" +
                                " through an unnecessarily long gauntlet of spikes and sawblades just to pick it up.";
                    Placement.AddVisitFlag(VisitState.Previewed);
                }
                else
                {
                    value = "A corpse in white armour. You already got the stuff it was holding.";
                }
            }
        }
    }
}
