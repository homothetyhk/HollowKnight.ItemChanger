using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Internal
{
    public static class Ref
    {
        public static Settings Settings => ItemChangerMod.SET;
        public static CustomSkills SKILLS => ItemChangerMod.SET.CustomSkills;
        public static WorldEvents WORLD => ItemChangerMod.SET.WorldEvents;
        public static PlayerData PD => PlayerData.instance;
        public static GameManager GM => GameManager.instance;
        public static HeroController HC => HeroController.instance;

        public static void QuickSave(params AbstractPlacement[] placements) => Settings.SavePlacements(placements);
    }
}
