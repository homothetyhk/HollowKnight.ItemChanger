using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using HutongGames.PlayMaker.Actions;

namespace ItemChanger.Internal
{
    internal static class ObjectCache
    {
        public static List<(string, string)> GetPreloadNames()
        {
            var preloads = new List<(string, string)>
            {
                (SceneNames.Tutorial_01, "_Props/Chest"),
                (SceneNames.Tutorial_01, "_Enemies/Crawler 1"),
                (SceneNames.Tutorial_01, "_Props/Cave Spikes (1)"),
                (SceneNames.Tutorial_01, "_Scenery/plat_float_17"),
                (SceneNames.Tutorial_01, "_Props/Tut_tablet_top (1)"),
                (SceneNames.Tutorial_01, "_Props/Geo Rock 1"),
                (SceneNames.Cliffs_02, "Soul Totem 5"),
                (SceneNames.Ruins_House_01, "Grub Bottle"),
            };
            if (!ItemChanger.GS.ReducePreloads)
            {
                preloads.AddRange(new List<(string, string)>
                {
                    (SceneNames.Abyss_19, "Geo Rock Abyss"),
                    (SceneNames.Ruins2_05, "Geo Rock City 1"),
                    (SceneNames.Deepnest_02, "Geo Rock Deepnest"),
                    (SceneNames.Fungus2_11, "Geo Rock Fung 01"),
                    (SceneNames.Fungus2_11, "Geo Rock Fung 02"),
                    (SceneNames.RestingGrounds_10, "Geo Rock Grave 01"),
                    (SceneNames.RestingGrounds_10, "Geo Rock Grave 02"),
                    (SceneNames.Fungus1_12, "Geo Rock Green Path 01"),
                    (SceneNames.Fungus1_12, "Geo Rock Green Path 02"),
                    (SceneNames.Hive_01, "Geo Rock Hive"),
                    (SceneNames.Mines_20, "Geo Rock Mine (4)"),
                    (SceneNames.Deepnest_East_17, "Geo Rock Outskirts"),
                    (SceneNames.Deepnest_East_17, "Giant Geo Egg")
                });
            }
            return preloads;
        }


        public static GameObject Chest => UnityEngine.Object.Instantiate(_chest);
        public static GameObject ShinyItem => UnityEngine.Object.Instantiate(_shinyItem);
        public static GameObject SmallGeo => UnityEngine.Object.Instantiate(_smallGeo);
        public static GameObject MediumGeo => UnityEngine.Object.Instantiate(_mediumGeo);
        public static GameObject LargeGeo => UnityEngine.Object.Instantiate(_largeGeo);
        public static GameObject TinkEffect => UnityEngine.Object.Instantiate(_tinkEffect);
        public static GameObject SmallPlatform => UnityEngine.Object.Instantiate(_smallPlatform);
        public static GameObject Soul => UnityEngine.Object.Instantiate(_soul);
        public static GameObject RelicGetMsg => UnityEngine.Object.Instantiate(_relicGetMsg);
        public static GameObject GrubJar => UnityEngine.Object.Instantiate(_grubJar);
        public static GameObject LoreTablet => UnityEngine.Object.Instantiate(_loreTablet);

        public static AudioClip LoreSound;
        public static AudioClip[] GrubCries;

        public static GeoRockSubtype GetPreloadedRockType(GeoRockSubtype t)
        {
            return _geoRocks.ContainsKey(t) ? t : GeoRockSubtype.Default;
        }

        public static GameObject GeoRock(GeoRockSubtype t)
        {
            return UnityEngine.Object.Instantiate(_geoRocks[GetPreloadedRockType(t)]);
        }

        public static void Setup(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            _chest = objectsByScene[SceneNames.Tutorial_01]["_Props/Chest"];
            _shinyItem = _chest.transform.Find("Item").Find("Shiny Item (1)").gameObject;
            _shinyItem.transform.parent = null;
            _shinyItem.name = "Shiny Item Mod";
            UnityEngine.Object.DontDestroyOnLoad(_chest);
            UnityEngine.Object.DontDestroyOnLoad(_shinyItem);
            PlayMakerFSM shinyFSM = _shinyItem.LocateFSM("Shiny Control");
            _relicGetMsg = UnityEngine.Object.Instantiate(shinyFSM.GetState("Trink Flash").GetActionsOfType<SpawnObjectFromGlobalPool>()[1].gameObject.Value);
            _relicGetMsg.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_relicGetMsg);

            HealthManager health = objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"].GetComponent<HealthManager>();
            _smallGeo = UnityEngine.Object.Instantiate(
                ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "smallGeoPrefab"));
            _mediumGeo =
                UnityEngine.Object.Instantiate(ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "mediumGeoPrefab"));
            _largeGeo = UnityEngine.Object.Instantiate(
                ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "largeGeoPrefab"));

            _smallGeo.SetActive(false);
            _mediumGeo.SetActive(false);
            _largeGeo.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_smallGeo);
            UnityEngine.Object.DontDestroyOnLoad(_mediumGeo);
            UnityEngine.Object.DontDestroyOnLoad(_largeGeo);

            PlayMakerFSM soulFsm = objectsByScene[SceneNames.Cliffs_02]["Soul Totem 5"].LocateMyFSM("soul_totem");
            _soul = UnityEngine.Object.Instantiate(soulFsm.GetState("Hit").GetActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value);
            _soul.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_soul);

            UnityEngine.Object.Destroy(objectsByScene[SceneNames.Tutorial_01]["_Props/Cave Spikes (1)"]);
            UnityEngine.Object.Destroy(objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"]);

            _tinkEffect = UnityEngine.Object.Instantiate(objectsByScene[SceneNames.Tutorial_01]["_Props/Cave Spikes (1)"].GetComponent<TinkEffect>().blockEffect);
            _tinkEffect.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_tinkEffect);

            _smallPlatform = objectsByScene[SceneNames.Tutorial_01]["_Scenery/plat_float_17"];
            UnityEngine.Object.DontDestroyOnLoad(_smallPlatform);

            _grubJar = objectsByScene[SceneNames.Ruins_House_01]["Grub Bottle"];
            GrubCries = _grubJar.transform.Find("Grub").gameObject.LocateMyFSM("Grub Control").GetState("Leave").GetActionOfType<AudioPlayRandom>().audioClips;
            UnityEngine.Object.DontDestroyOnLoad(_grubJar);
            foreach (AudioClip clip in GrubCries)
            {
                UnityEngine.Object.DontDestroyOnLoad(clip);
            }

            _loreTablet = objectsByScene[SceneNames.Tutorial_01]["_Props/Tut_tablet_top (1)"];
            _loreTablet.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            LoreSound = (AudioClip)_loreTablet.LocateMyFSM("Inspection").GetState("Prompt Up").GetActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            UnityEngine.Object.DontDestroyOnLoad(LoreSound);
            UnityEngine.Object.DontDestroyOnLoad(_loreTablet);

            if (ItemChanger.GS.ReducePreloads)
            {
                _geoRocks = new Dictionary<GeoRockSubtype, GameObject>()
                {
                    [GeoRockSubtype.Default] = objectsByScene[SceneNames.Tutorial_01]["_Props/Geo Rock 1"],
                };
            }
            else
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
            }

            foreach (var entry in _geoRocks)
            {
                UnityEngine.Object.DontDestroyOnLoad(entry.Value);
            }
        }

        private static GameObject _chest;
        private static GameObject _shinyItem;
        private static GameObject _smallGeo;
        private static GameObject _mediumGeo;
        private static GameObject _largeGeo;
        private static GameObject _tinkEffect;
        private static GameObject _smallPlatform;
        private static GameObject _grubJar;
        private static GameObject _soul;
        private static GameObject _relicGetMsg;
        private static GameObject _loreTablet;
        private static Dictionary<GeoRockSubtype, GameObject> _geoRocks;
    }
}
