using SeanprCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using HutongGames.PlayMaker.Actions;

namespace ItemChanger
{
    internal static class ObjectCache
    {
        public static List<(string, string)> preloads = new List<(string, string)>
        {
            (SceneNames.Tutorial_01, "_Props/Chest/Item/Shiny Item (1)"),
            (SceneNames.Tutorial_01, "_Props/Cave Spikes (1)"),
            (SceneNames.Tutorial_01, "_Enemies/Crawler 1"),
            (SceneNames.Tutorial_01, "_Scenery/plat_float_17"),
            (SceneNames.Ruins_House_01, "Grub Bottle/Grub"),
        };

        

        public static GameObject ShinyItem => GameObject.Instantiate(_shinyItem);

        public static GameObject SmallGeo => GameObject.Instantiate(_smallGeo);

        public static GameObject MediumGeo => GameObject.Instantiate(_mediumGeo);

        public static GameObject LargeGeo => GameObject.Instantiate(_largeGeo);

        public static GameObject TinkEffect => GameObject.Instantiate(_tinkEffect);

        public static GameObject SmallPlatform => GameObject.Instantiate(_smallPlatform);

        public static void Setup(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            _shinyItem = preloadedObjects[SceneNames.Tutorial_01]["_Props/Chest/Item/Shiny Item (1)"];
            _shinyItem.name = "Shiny Item Mod";
            GameObject.DontDestroyOnLoad(_shinyItem);

            HealthManager health = preloadedObjects[SceneNames.Tutorial_01]["_Enemies/Crawler 1"].GetComponent<HealthManager>();
            _smallGeo = GameObject.Instantiate(
                ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "smallGeoPrefab"));
            _mediumGeo =
                GameObject.Instantiate(ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "mediumGeoPrefab"));
            _largeGeo = GameObject.Instantiate(
                ReflectionHelper.GetAttr<HealthManager, GameObject>(health, "largeGeoPrefab"));

            _smallGeo.SetActive(false);
            _mediumGeo.SetActive(false);
            _largeGeo.SetActive(false);
            GameObject.DontDestroyOnLoad(_smallGeo);
            GameObject.DontDestroyOnLoad(_mediumGeo);
            GameObject.DontDestroyOnLoad(_largeGeo);

            _tinkEffect = GameObject.Instantiate(preloadedObjects[SceneNames.Tutorial_01]["_Props/Cave Spikes (1)"].GetComponent<TinkEffect>().blockEffect);
            _tinkEffect.SetActive(false);
            GameObject.DontDestroyOnLoad(_tinkEffect);

            _smallPlatform = preloadedObjects[SceneNames.Tutorial_01]["_Scenery/plat_float_17"];
            GameObject.DontDestroyOnLoad(_smallPlatform);

            _grub = preloadedObjects[SceneNames.Ruins_House_01]["Grub Bottle/Grub"];
            GrubCries = _grub.LocateMyFSM("Grub Control").GetState("Leave").GetActionOfType<AudioPlayRandom>().audioClips;
            GameObject.DontDestroyOnLoad(_grub);
            foreach (AudioClip clip in GrubCries)
            {
                GameObject.DontDestroyOnLoad(clip);
            }
        }

        private static GameObject _shinyItem;
        private static GameObject _smallGeo;
        private static GameObject _mediumGeo;
        private static GameObject _largeGeo;
        private static GameObject _tinkEffect;
        private static GameObject _smallPlatform;
        private static GameObject _grub;
        public static AudioClip[] GrubCries;
    }
}
