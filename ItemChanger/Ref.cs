using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public static class Ref
    {
        public static Settings Placements => ItemChanger.instance.SET;
        public static CustomSkills SKILLS => ItemChanger.instance.SET.CustomSkills;
        public static PlayerData PD => PlayerData.instance;
        public static GameManager GM => GameManager.instance;
        public static HeroController HC => HeroController.instance;
    }
}
