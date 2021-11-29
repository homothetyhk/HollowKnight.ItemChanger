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
                        _smallGeo = UObject.Instantiate(o.prefab);
                        _smallGeo.SetActive(false);
                        UObject.DontDestroyOnLoad(_smallGeo);
                        break;
                    case "Geo Med":
                        _mediumGeo = UObject.Instantiate(o.prefab);
                        _mediumGeo.SetActive(false);
                        UObject.DontDestroyOnLoad(_mediumGeo);
                        break;
                    case "Geo Large":
                        _largeGeo = UObject.Instantiate(o.prefab);
                        _largeGeo.SetActive(false);
                        UObject.DontDestroyOnLoad(_largeGeo);
                        break;
                    case "Soul Orb R":
                        _soulOrb = UObject.Instantiate(o.prefab);
                        _soulOrb.GetComponent<AudioSource>().priority = 200;
                        // this guarantees that when we spawn several soul orbs,
                        // they don't force other audio sources to become virtual and stop playing sound
                        // normally, this has the default priority 128 (on a [0, 255] scale)
                        _soulOrb.SetActive(false);
                        UObject.DontDestroyOnLoad(_soulOrb);
                        break;
                }
            }

        }

        private static GameObject _smallGeo;
        private static GameObject _mediumGeo;
        private static GameObject _largeGeo;
        private static GameObject _soulOrb;

        public static GameObject SmallGeo => UObject.Instantiate(_smallGeo);
        public static GameObject MediumGeo => UObject.Instantiate(_mediumGeo);
        public static GameObject LargeGeo => UObject.Instantiate(_largeGeo);
        public static GameObject SoulOrb => UObject.Instantiate(_soulOrb);


        public static GameObject Chest => CorePreloader.Chest;
        public static GameObject ShinyItem => CorePreloader.ShinyItem;
        public static GameObject SmallPlatform => CorePreloader.SmallPlatform;
        public static GameObject RelicGetMsg => CorePreloader.RelicGetMsg;
        public static GameObject GrubJar => GrubPreloader.GrubJar;
        public static GameObject MimicBottle => MimicPreloader.MimicBottle;
        public static GameObject MimicTop => MimicPreloader.MimicTop;
        public static GameObject LumaflyEscape => CorePreloader.LumaflyEscape;
        public static GameObject LoreTablet => CorePreloader.LoreTablet;
        public static GameObject GeoRock(ref GeoRockSubtype t) => GeoRockPreloader.GeoRock(ref t);
        public static GameObject SoulTotem(SoulTotemSubtype t) => SoulTotemPreloader.SoulTotem(t);
        public static GameObject SoulParticles => SoulTotemPreloader.SoulParticles;
    }
}
