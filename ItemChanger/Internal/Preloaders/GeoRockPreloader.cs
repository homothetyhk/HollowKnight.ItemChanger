namespace ItemChanger.Internal.Preloaders
{
    public class GeoRockPreloader : Preloader
    {
        public override IEnumerable<(string, string)> GetPreloadNames()
        {
            if (PreloadLevel != PreloadLevel.None)
            {
                yield return (SceneNames.Tutorial_01, "_Props/Geo Rock 1");
            }
            if (PreloadLevel == PreloadLevel.Full)
            {
                yield return (SceneNames.Abyss_19, "Geo Rock Abyss");
                yield return (SceneNames.Ruins2_05, "Geo Rock City 1");
                yield return (SceneNames.Deepnest_02, "Geo Rock Deepnest");
                yield return (SceneNames.Fungus2_11, "Geo Rock Fung 01");
                yield return (SceneNames.Fungus2_11, "Geo Rock Fung 02");
                yield return (SceneNames.RestingGrounds_10, "Geo Rock Grave 01");
                yield return (SceneNames.RestingGrounds_10, "Geo Rock Grave 02");
                yield return (SceneNames.Fungus1_12, "Geo Rock Green Path 01");
                yield return (SceneNames.Fungus1_12, "Geo Rock Green Path 02");
                yield return (SceneNames.Hive_01, "Geo Rock Hive");
                yield return (SceneNames.Mines_20, "Geo Rock Mine (4)");
                yield return (SceneNames.Deepnest_East_17, "Geo Rock Outskirts");
                yield return (SceneNames.Deepnest_East_17, "Giant Geo Egg");
            }
        }

        public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            if (PreloadLevel == PreloadLevel.Full)
            {
                _geoRocks = new Dictionary<GeoRockSubtype, GameObject>()
                {
                    [GeoRockSubtype.Default] = objectsByScene[SceneNames.Tutorial_01]["_Props/Geo Rock 1"],
                    [GeoRockSubtype.Abyss] = objectsByScene[SceneNames.Abyss_19]["Geo Rock Abyss"],
                    [GeoRockSubtype.City] = objectsByScene[SceneNames.Ruins2_05]["Geo Rock City 1"],
                    [GeoRockSubtype.Deepnest] = objectsByScene[SceneNames.Deepnest_02]["Geo Rock Deepnest"],
                    [GeoRockSubtype.Fung01] = objectsByScene[SceneNames.Fungus2_11]["Geo Rock Fung 01"],
                    [GeoRockSubtype.Fung02] = objectsByScene[SceneNames.Fungus2_11]["Geo Rock Fung 02"],
                    [GeoRockSubtype.Grave01] = objectsByScene[SceneNames.RestingGrounds_10]["Geo Rock Grave 01"],
                    [GeoRockSubtype.Grave02] = objectsByScene[SceneNames.RestingGrounds_10]["Geo Rock Grave 02"],
                    [GeoRockSubtype.GreenPath01] = objectsByScene[SceneNames.Fungus1_12]["Geo Rock Green Path 01"],
                    [GeoRockSubtype.GreenPath02] = objectsByScene[SceneNames.Fungus1_12]["Geo Rock Green Path 02"],
                    [GeoRockSubtype.Hive] = objectsByScene[SceneNames.Hive_01]["Geo Rock Hive"],
                    [GeoRockSubtype.Mine] = objectsByScene[SceneNames.Mines_20]["Geo Rock Mine (4)"],
                    [GeoRockSubtype.Outskirts] = objectsByScene[SceneNames.Deepnest_East_17]["Geo Rock Outskirts"],
                    [GeoRockSubtype.Outskirts420] = objectsByScene[SceneNames.Deepnest_East_17]["Giant Geo Egg"]
                };

                foreach (var entry in _geoRocks)
                {
                    UObject.DontDestroyOnLoad(entry.Value);
                }
            }
            else if (PreloadLevel == PreloadLevel.Reduced)
            {
                _geoRocks = new Dictionary<GeoRockSubtype, GameObject>()
                {
                    [GeoRockSubtype.Default] = objectsByScene[SceneNames.Tutorial_01]["_Props/Geo Rock 1"],
                };

                foreach (var entry in _geoRocks)
                {
                    UObject.DontDestroyOnLoad(entry.Value);
                }
            }
        }

        public GeoRockSubtype GetPreloadedRockType(GeoRockSubtype t)
        {
            return _geoRocks.ContainsKey(t) ? t : GeoRockSubtype.Default;
        }

        public GameObject GeoRock(GeoRockSubtype t)
        {
            if (PreloadLevel == PreloadLevel.None) throw NotPreloadedException();
            return UObject.Instantiate(_geoRocks[GetPreloadedRockType(t)]);
        }

        private Dictionary<GeoRockSubtype, GameObject> _geoRocks;
    }
}
