using ItemChanger.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using HutongGames.PlayMaker.Actions;
using UObject = UnityEngine.Object;
using ItemChanger.Internal.Preloaders;

namespace ItemChanger.Internal
{
    public static class ObjectCache
    {
        public static CorePreloader CorePreloader;
        public static GrubPreloader GrubPreloader;
        public static GeoRockPreloader GeoRockPreloader;
        public static MimicPreloader MimicPreloader;
        public static SoulTotemPreloader SoulTotemPreloader;
        private static bool _initialized = false;

        private static IEnumerable<Preloader> Preloaders
        {
            get
            {
                yield return CorePreloader;
                yield return GrubPreloader;
                yield return GeoRockPreloader;
                yield return MimicPreloader;
                yield return SoulTotemPreloader;
            }
        }

        private static void Initialize()
        {
            _initialized = true;
            CorePreloader = new() { PreloadLevel = PreloadLevel.Full };
            GrubPreloader = new() { PreloadLevel = ItemChangerMod.GS.PreloadSettings.PreloadGrub };
            GeoRockPreloader = new() { PreloadLevel = ItemChangerMod.GS.PreloadSettings.PreloadGeoRocks };
            MimicPreloader = new() { PreloadLevel = ItemChangerMod.GS.PreloadSettings.PreloadMimic };
            SoulTotemPreloader = new() { PreloadLevel = ItemChangerMod.GS.PreloadSettings.PreloadSoulTotems };
        }

        public static List<(string, string)> GetPreloadNames()
        {
            if (!_initialized) Initialize();
            return Preloaders.SelectMany(p => p.GetPreloadNames()).ToList();
        }

        public static void Setup(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            foreach (Preloader p in Preloaders) p.SavePreloads(objectsByScene);
            foreach (var o in ObjectPool.instance.startupPools)
            {
                switch (o.prefab.name)
                {
                    case "Geo Small":
                        SmallGeoPrefab = o.prefab;
                        break;
                    case "Geo Medium":
                        MediumGeoPrefab = o.prefab;
                        break;
                    case "Geo Large":
                        LargeGeoPrefab = o.prefab;
                        break;
                    case "Soul Orb R":
                        SoulOrbPrefab = o.prefab;
                        o.prefab.GetComponent<AudioSource>().priority = 200;
                        // kind of bad to change a shared prefab, but this guarantees
                        // that when we spawn several soul orbs, they don't force other audio sources to become virtual and stop playing sound
                        // normally, this has the default priority 128 (on a [0, 255] scale)
                        break;
                }
            }

        }

        public static GameObject SmallGeoPrefab { get; private set; }
        public static GameObject MediumGeoPrefab { get; private set; }
        public static GameObject LargeGeoPrefab { get; private set; }
        public static GameObject SoulOrbPrefab { get; private set; }


        public static GameObject Chest => CorePreloader.Chest;
        public static GameObject ShinyItem => CorePreloader.ShinyItem;
        public static GameObject SmallPlatform => CorePreloader.SmallPlatform;
        public static GameObject RelicGetMsg => CorePreloader.RelicGetMsg;
        public static GameObject GrubJar => GrubPreloader.GrubJar;
        public static GameObject MimicBottle => MimicPreloader.MimicBottle;
        public static GameObject MimicTop => MimicPreloader.MimicTop;
        public static GameObject LumaflyEscape => CorePreloader.LumaflyEscape;
        public static GameObject LoreTablet => CorePreloader.LoreTablet;
        public static GameObject GeoRock(GeoRockSubtype t) => GeoRockPreloader.GeoRock(t);
        public static GameObject SoulTotem(SoulTotemSubtype t) => SoulTotemPreloader.SoulTotem(t);
    }
}
