namespace ItemChanger.Util
{
    public static class SceneUtil
    {
        static Dictionary<string, string> subScenes = new Dictionary<string, string>
        {
            { SceneNames.Crossroads_10_preload, SceneNames.Crossroads_10 },
            { SceneNames.Crossroads_10_boss, SceneNames.Crossroads_10 },
            { SceneNames.Crossroads_10_boss_defeated, SceneNames.Crossroads_10 },
            { SceneNames.Ruins1_24_boss, SceneNames.Ruins1_24 },
            { SceneNames.Ruins1_24_boss_defeated, SceneNames.Ruins1_24 },
            { SceneNames.Ruins2_03_boss, SceneNames.Ruins2_03 },
            { SceneNames.Ruins2_11_boss, SceneNames.Ruins2_11 },
            { SceneNames.Fungus1_04_boss, SceneNames.Fungus1_04 },
            { SceneNames.Fungus2_15_boss, SceneNames.Fungus2_15 },
            { SceneNames.Fungus2_15_boss_defeated, SceneNames.Fungus2_15 },
            { SceneNames.Fungus3_23_boss, SceneNames.Fungus3_23 },
            { SceneNames.Fungus3_40_boss, SceneNames.Fungus3_40 },
            { SceneNames.Fungus3_archive_02_boss, SceneNames.Fungus3_archive_02 },
            { SceneNames.Cliffs_02_boss, SceneNames.Cliffs_02 },
            { SceneNames.RestingGrounds_02_boss, SceneNames.RestingGrounds_02 },
            { SceneNames.Mines_18_boss, SceneNames.Mines_18 },
            { SceneNames.Deepnest_East_Hornet_boss, SceneNames.Deepnest_East_Hornet },
            { SceneNames.Waterways_05_boss, SceneNames.Waterways_05 },
            { SceneNames.Waterways_12_boss, SceneNames.Waterways_12 },
            { SceneNames.Grimm_Main_Tent_boss, SceneNames.Grimm_Main_Tent },
        };

        public static bool TryGetSuperScene(string subScene, out string superScene)
        {
            return subScenes.TryGetValue(subScene, out superScene);
        }

        public static bool IsSubscene(string subscene, string scene)
        {
            if (!subscene.StartsWith(scene)) return false;
            if (subscene == scene) return true;
            return subscene.EndsWith("_boss") || subscene.EndsWith("_boss_defeated") || subscene.EndsWith("_preload");
        }


    }
}
