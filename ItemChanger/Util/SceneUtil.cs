using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Extensions;

namespace ItemChanger.Util
{
    public static class SceneUtil
    {
        /*
        static Dictionary<string, string> subScenes = new Dictionary<string, string>
        {
            { "Crossroads_10_preload", SceneNames.Crossroads_10 },
            { "Crossroads_10_boss", SceneNames.Crossroads_10 },
            { "Crossroads_10_boss_defeated", SceneNames.Crossroads_10 },
            { "Ruins1_24_boss", SceneNames.Ruins1_24 },
            { "Ruins1_24_boss_defeated", SceneNames.Ruins1_24 },
            { "Ruins2_03_boss", SceneNames.Ruins2_03 },
            { "Ruins2_11_boss", SceneNames.Ruins2_11 },
            { "Fungus1_04_boss", SceneNames.Fungus1_04 },
            { "Fungus2_15_boss", SceneNames.Fungus2_15 },
            { "Fungus2_15_boss_defeated", SceneNames.Fungus2_15 },
            { "Fungus3_23_boss", SceneNames.Fungus3_23 },
            { "Fungus3_40_boss", SceneNames.Fungus3_40 },
            { "Fungus3_archive_02_boss", SceneNames.Fungus3_archive_02 },
            { "Cliffs_02_boss", SceneNames.Cliffs_02 },
            { "RestingGrounds_02_boss", SceneNames.RestingGrounds_02 },
            { "Mines_18_boss", SceneNames.Mines_18 },
            { "Fungus2_15_boss", SceneNames.Fungus2_15 },
            { "Deepnest_East_Hornet_boss", SceneNames.Deepnest_East_Hornet },
            { "Waterways_05_boss", SceneNames.Waterways_05 },
            { "Waterways_12_boss", SceneNames.Waterways_12 },
            { "Grimm_Main_Tent_boss", SceneNames.Grimm_Main_Tent },
        };
        */

        public static bool IsSubscene(string subscene, string scene)
        {
            if (!subscene.StartsWith(scene)) return false;
            if (subscene == scene) return true;
            return subscene.EndsWith("_boss") || subscene.EndsWith("_boss_defeated") || subscene.EndsWith("_preload");
        }


    }
}
