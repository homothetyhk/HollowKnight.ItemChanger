using ItemChanger.Extensions;
using HutongGames.PlayMaker.Actions;

namespace ItemChanger.Internal.Preloaders
{
    public class CorePreloader : Preloader
    {
        public override IEnumerable<(string, string)> GetPreloadNames()
        {
            yield return (SceneNames.Tutorial_01, "_Props/Chest");
            yield return (SceneNames.Tutorial_01, "_Scenery/plat_float_17");
            yield return (SceneNames.Tutorial_01, "_Props/Tut_tablet_top (1)");
            yield return (SceneNames.Deepnest_36, "d_break_0047_deep_lamp2/lamp_bug_escape (7)");
            yield return (SceneNames.Ruins1_05b, "Shop Menu");
        }

        public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            _chest = objectsByScene[SceneNames.Tutorial_01]["_Props/Chest"];
            _shinyItem = _chest.transform.Find("Item").Find("Shiny Item (1)").gameObject;
            _shinyItem.transform.parent = null;
            _shinyItem.name = "Shiny Item Mod";
            UObject.DontDestroyOnLoad(_chest);
            UObject.DontDestroyOnLoad(_shinyItem);
            PlayMakerFSM shinyFSM = _shinyItem.LocateMyFSM("Shiny Control");
            _relicGetMsg = UObject.Instantiate(shinyFSM.GetState("Trink Flash").GetActionsOfType<SpawnObjectFromGlobalPool>()[1].gameObject.Value);
            _relicGetMsg.SetActive(false);
            UObject.DontDestroyOnLoad(_relicGetMsg);

            _smallPlatform = objectsByScene[SceneNames.Tutorial_01]["_Scenery/plat_float_17"];
            UObject.DontDestroyOnLoad(_smallPlatform);

            _lumaflyEscape = objectsByScene[SceneNames.Deepnest_36]["d_break_0047_deep_lamp2/lamp_bug_escape (7)"];
            FixLumaflyEscape(_lumaflyEscape);
            UObject.DontDestroyOnLoad(_lumaflyEscape);

            _loreTablet = objectsByScene[SceneNames.Tutorial_01]["_Props/Tut_tablet_top (1)"];
            _loreTablet.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            UObject.DontDestroyOnLoad(_loreTablet);

            _shopMenu = objectsByScene[SceneNames.Ruins1_05b]["Shop Menu"];
            ShopMenuStock shop = _shopMenu.GetComponent<ShopMenuStock>();
            _shopItem = UObject.Instantiate(shop.stock[0]);
            GameObject amountIndicator = _shopItem.transform.Find("Amount").gameObject;
            UnityEngine.Object.Destroy(amountIndicator);
            _shopItem.SetActive(false);
            PatchShop(_shopMenu);
            UObject.DontDestroyOnLoad(_shopMenu);
            UObject.DontDestroyOnLoad(_shopItem);
        }

        public GameObject Chest => UObject.Instantiate(_chest);
        public GameObject ShinyItem => UObject.Instantiate(_shinyItem);
        public GameObject SmallPlatform => UObject.Instantiate(_smallPlatform);
        public GameObject RelicGetMsg => UObject.Instantiate(_relicGetMsg);
        public GameObject LoreTablet => UObject.Instantiate(_loreTablet);
        public GameObject LumaflyEscape => UObject.Instantiate(_lumaflyEscape);
        public GameObject ShopMenu => UObject.Instantiate(_shopMenu);
        public GameObject ShopItem => UObject.Instantiate(_shopItem);

        private GameObject _chest;
        private GameObject _shinyItem;
        private GameObject _relicGetMsg;
        private GameObject _smallPlatform;
        private GameObject _loreTablet;
        private GameObject _lumaflyEscape;
        private GameObject _shopMenu;
        private GameObject _shopItem;


        private static void FixLumaflyEscape(GameObject lumaflyEscape)
        {
            ParticleSystem.MainModule psm = lumaflyEscape.GetComponent<ParticleSystem>().main;
            ParticleSystem.EmissionModule pse = lumaflyEscape.GetComponent<ParticleSystem>().emission;
            ParticleSystem.ShapeModule pss = lumaflyEscape.GetComponent<ParticleSystem>().shape;
            ParticleSystem.TextureSheetAnimationModule pst = lumaflyEscape.GetComponent<ParticleSystem>().textureSheetAnimation;
            ParticleSystem.ForceOverLifetimeModule psf = lumaflyEscape.GetComponent<ParticleSystem>().forceOverLifetime;

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

            Transform t = lumaflyEscape.GetComponent<Transform>();
            Vector3 loc = t.localScale;
            loc.x = 1f;
            t.localScale = loc;
        }

        private static void PatchShop(GameObject shopMenu)
        {
            shopMenu.transform.Find("Item Details").gameObject.SetActive(true);
            shopMenu.transform.Find("Confirm").Find("Confirm msg")
                .GetComponent<SetTextMeshProGameText>().convName = "SHOP_PURCHASE_CONFIRM";
            shopMenu.SetActive(false);
        }
    }
}
