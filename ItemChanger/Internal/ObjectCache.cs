using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using HutongGames.PlayMaker.Actions;
using UObject = UnityEngine.Object;

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
                (SceneNames.Deepnest_36, "Grub Bottle"),
                (SceneNames.Deepnest_36, "Grub Mimic Bottle"),
                (SceneNames.Deepnest_36, "Grub Mimic Top"),
                //(SceneNames.Deepnest_36, "Dream Dialogue"),
                (SceneNames.Deepnest_36, "d_break_0047_deep_lamp2/lamp_bug_escape (7)"),
                // extra geo rocks
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
                (SceneNames.Deepnest_East_17, "Giant Geo Egg"),
            };
            return preloads;
        }


        public static GameObject Chest => UObject.Instantiate(_chest);
        public static GameObject ShinyItem => UObject.Instantiate(_shinyItem);
        public static GameObject SmallGeo => UObject.Instantiate(_smallGeo);
        public static GameObject MediumGeo => UObject.Instantiate(_mediumGeo);
        public static GameObject LargeGeo => UObject.Instantiate(_largeGeo);
        public static GameObject SmallPlatform => UObject.Instantiate(_smallPlatform);
        public static GameObject Soul => UObject.Instantiate(_soul);
        public static GameObject RelicGetMsg => UObject.Instantiate(_relicGetMsg);
        public static GameObject GrubJar => UObject.Instantiate(_grubJar);
        public static GameObject MimicBottle => UObject.Instantiate(_mimicBottle);
        public static GameObject MimicTop => UObject.Instantiate(_mimicTop);
        public static GameObject MimicDreamDialogue => UObject.Instantiate(_mimicDialogue);
        public static GameObject LumaflyEscape => UObject.Instantiate(_lumaflyEscape);
        public static GameObject LoreTablet => UObject.Instantiate(_loreTablet);


        public static AudioClip LoreSound;
        public static AudioClip[] GrubCries;
        public static AudioClip MimicScream;
        public static AudioClip BigItemJingle; // TODO: add this to BigItemPopup

        public static GeoRockSubtype GetPreloadedRockType(GeoRockSubtype t)
        {
            return _geoRocks.ContainsKey(t) ? t : GeoRockSubtype.Default;
        }

        public static GameObject GeoRock(GeoRockSubtype t)
        {
            return UObject.Instantiate(_geoRocks[GetPreloadedRockType(t)]);
        }

        public static void Setup(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            _chest = objectsByScene[SceneNames.Tutorial_01]["_Props/Chest"];
            _shinyItem = _chest.transform.Find("Item").Find("Shiny Item (1)").gameObject;
            _shinyItem.transform.parent = null;
            _shinyItem.name = "Shiny Item Mod";
            UObject.DontDestroyOnLoad(_chest);
            UObject.DontDestroyOnLoad(_shinyItem);
            PlayMakerFSM shinyFSM = _shinyItem.LocateFSM("Shiny Control");
            _relicGetMsg = UObject.Instantiate(shinyFSM.GetState("Trink Flash").GetActionsOfType<SpawnObjectFromGlobalPool>()[1].gameObject.Value);
            _relicGetMsg.SetActive(false);
            UObject.DontDestroyOnLoad(_relicGetMsg);

            BigItemJingle = (AudioClip)(shinyFSM.GetState("Walljump")
                .GetFirstActionOfType<CreateUIMsgGetItem>().gameObject.Value
                .LocateMyFSM("Msg Control")
                .GetState("Top Up")
                .GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value);
            UObject.DontDestroyOnLoad(BigItemJingle);

            HealthManager health = objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"].GetComponent<HealthManager>();
            _smallGeo = UObject.Instantiate(
                ReflectionHelper.GetField<HealthManager, GameObject>(health, "smallGeoPrefab"));
            _mediumGeo =
                UObject.Instantiate(ReflectionHelper.GetField<HealthManager, GameObject>(health, "mediumGeoPrefab"));
            _largeGeo = UObject.Instantiate(
                ReflectionHelper.GetField<HealthManager, GameObject>(health, "largeGeoPrefab"));

            _smallGeo.SetActive(false);
            _mediumGeo.SetActive(false);
            _largeGeo.SetActive(false);
            UObject.DontDestroyOnLoad(_smallGeo);
            UObject.DontDestroyOnLoad(_mediumGeo);
            UObject.DontDestroyOnLoad(_largeGeo);

            PlayMakerFSM soulFsm = objectsByScene[SceneNames.Cliffs_02]["Soul Totem 5"].LocateMyFSM("soul_totem");
            _soul = UObject.Instantiate(soulFsm.GetState("Hit").GetFirstActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value);
            _soul.SetActive(false);
            UObject.DontDestroyOnLoad(_soul);

            UObject.Destroy(objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"]);

            _smallPlatform = objectsByScene[SceneNames.Tutorial_01]["_Scenery/plat_float_17"];
            UObject.DontDestroyOnLoad(_smallPlatform);

            _grubJar = objectsByScene[SceneNames.Deepnest_36]["Grub Bottle"];
            GrubCries = _grubJar.transform.Find("Grub").gameObject.LocateMyFSM("Grub Control").GetState("Leave").GetFirstActionOfType<AudioPlayRandom>().audioClips;
            UObject.DontDestroyOnLoad(_grubJar);
            foreach (AudioClip clip in GrubCries)
            {
                UObject.DontDestroyOnLoad(clip);
            }

            _mimicBottle = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Bottle"];
            UObject.DontDestroyOnLoad(_mimicBottle);
            _mimicTop = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Top"];
            UObject.DontDestroyOnLoad(_mimicTop);
            MimicScream = (AudioClip)_mimicTop.transform.Find("Grub Mimic 1").gameObject.LocateMyFSM("Grub Mimic").GetState("Scream")
                .GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            UObject.DontDestroyOnLoad(MimicScream);
            /*
            _mimicDialogue = objectsByScene[SceneNames.Deepnest_36]["Dream Dialogue"];
            UObject.DontDestroyOnLoad(_mimicDialogue);
            */

            _lumaflyEscape = objectsByScene[SceneNames.Deepnest_36]["d_break_0047_deep_lamp2/lamp_bug_escape (7)"];
            FixLumaflyEscape();
            UObject.DontDestroyOnLoad(_lumaflyEscape);

            _loreTablet = objectsByScene[SceneNames.Tutorial_01]["_Props/Tut_tablet_top (1)"];
            _loreTablet.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            LoreSound = (AudioClip)_loreTablet.LocateMyFSM("Inspection").GetState("Prompt Up").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            UObject.DontDestroyOnLoad(LoreSound);
            UObject.DontDestroyOnLoad(_loreTablet);

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

        private static GameObject _chest;
        private static GameObject _shinyItem;
        private static GameObject _smallGeo;
        private static GameObject _mediumGeo;
        private static GameObject _largeGeo;
        private static GameObject _smallPlatform;
        private static GameObject _grubJar;
        private static GameObject _mimicBottle;
        private static GameObject _mimicTop;
        private static GameObject _mimicDialogue;
        private static GameObject _lumaflyEscape;
        private static GameObject _soul;
        private static GameObject _relicGetMsg;
        private static GameObject _loreTablet;
        private static Dictionary<GeoRockSubtype, GameObject> _geoRocks;

        private static void FixLumaflyEscape()
        {
            ParticleSystem.MainModule psm = _lumaflyEscape.GetComponent<ParticleSystem>().main;
            ParticleSystem.EmissionModule pse = _lumaflyEscape.GetComponent<ParticleSystem>().emission;
            ParticleSystem.ShapeModule pss = _lumaflyEscape.GetComponent<ParticleSystem>().shape;
            ParticleSystem.TextureSheetAnimationModule pst = _lumaflyEscape.GetComponent<ParticleSystem>().textureSheetAnimation;
            ParticleSystem.ForceOverLifetimeModule psf = _lumaflyEscape.GetComponent<ParticleSystem>().forceOverLifetime;

            psm.duration = 1f;
            psm.startLifetimeMultiplier = 4f;
            psm.startSizeMultiplier = 2f;
            psm.startSizeXMultiplier = 2f;
            psm.gravityModifier = -0.2f;
            psm.maxParticles = 99;              // In practice it only spawns 9 lumaflies
            pse.rateOverTimeMultiplier = 10f;
            pss.radius = 0.5868902f;
            pst.cycleCount = 15;
            psf.xMultiplier = 3;
            psf.yMultiplier = 8;

            // I have no idea what this is supposed to be lmao
            AnimationCurve yMax = new AnimationCurve(new Keyframe(0, 0.0810811371f), new Keyframe(0.230769232f, 0.108108163f),
                new Keyframe(0.416873455f, -0.135135055f), new Keyframe(0.610421836f, -0.054053992f), new Keyframe(0.799007416f, -0.29729721f));
            AnimationCurve yMin = new AnimationCurve(new Keyframe(0, 0.486486584f), new Keyframe(0.220843673f, 0.567567647f),
                new Keyframe(0.411910683f, 0.270270377f), new Keyframe(0.605459034f, 0.405405462f), new Keyframe(0.801488876f, 0.108108193f));
            psf.y = new ParticleSystem.MinMaxCurve(8, yMin, yMax);

            psf.x.curveMax.keys[0].value = -0.324324369f;
            psf.x.curveMax.keys[1].value = -0.432432413f;

            psf.x.curveMin.keys[0].value = 0.162162244f;
            psf.x.curveMin.keys[1].time = 0.159520522f;
            psf.x.curveMin.keys[1].value = 0.35135144f;

            Transform t = _lumaflyEscape.GetComponent<Transform>();
            Vector3 loc = t.localScale;
            loc.x = 1f;
            t.localScale = loc;
        }

    }
}
