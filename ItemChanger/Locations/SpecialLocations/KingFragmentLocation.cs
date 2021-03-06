﻿using ItemChanger.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    public class KingFragmentLocation : ObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            obj.GetOrAddComponent<ContainerInfo>().changeSceneInfo
                = new ChangeSceneInfo { toScene = "Abyss_05" };
            base.PlaceContainer(obj, containerType);
        }

        public override string OnLanguageGet(string convo, string sheet)
        {
            if (HintActive && sheet == "Lore Tablets" && convo == "DUSK_KNIGHT_CORPSE")
            {
                string item = Placement.GetUIName();
                if (!string.IsNullOrEmpty(item))
                {
                    return "A corpse in white armour. You can clearly see the "
                                + Placement.GetUIName() + " it's holding, " +
                                "but for some reason you get the feeling you're going to have to go" +
                                " through an unnecessarily long gauntlet of spikes and sawblades just to pick it up.";
                }
                else
                {
                    return "A corpse in white armour. You already got the stuff it was holding.";
                }
            }
            return base.OnLanguageGet(convo, sheet);
        }
    }
}
