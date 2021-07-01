using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using HutongGames.PlayMaker;
using ItemChanger.UIDefs;
using Modding;
using MonoMod;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItemChanger.Components;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Util;
using TMPro;

namespace ItemChanger
{
    public class ItemChanger : Mod
    {
        internal static ItemChanger instance;
        internal static bool readyForChangeItems = false;
        internal static bool receivedChangeItemsRequest = false;
        internal static bool receivedChangeStartRequest = false;

        public SaveSettings Settings { get; set; } = new SaveSettings();
        public ModSettings _SaveSettings
        {
            get => Settings = Settings;
            set => Settings = value as SaveSettings;
        }


        public Settings SET = new Settings();
        public override ModSettings SaveSettings
        {
            get => SET = SET ?? new Settings();
            set => SET = value as Settings;
        }

        private GlobalSettings _globalSettings { get; set; } = new GlobalSettings();
        public override ModSettings GlobalSettings { get => _globalSettings; set => _globalSettings = value as GlobalSettings; }
        public static GlobalSettings GS => instance._globalSettings;

        public ItemChanger()
        {
            instance = this;
            SpriteManager.Setup();
            //XmlManager.Load();
            LanguageStringManager.Load();
            Finder.Load();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ObjectCache.Setup(preloadedObjects);
            //readyForChangeItems = true;
            MessageController.Setup();
            

            Tests.Tests.FinderTest();

            On.GameManager.ResetSemiPersistentItems += ResetSemiPersistentItems;
            CustomSkillManager.Hook();
            WorldEventManager.Hook();
            Util.ShopUtil.HookShops();
            On.PlayMakerFSM.OnEnable += ApplyLocationFsmEdits;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
            On.GameManager.OnNextLevelReady += OnOnNextLevelReady;
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += OnNotifyFetchComplete;
            ModHooks.Instance.LanguageGetHook += OnLanguageGet;
            foreach (var loc in SET.GetPlacements()) loc.OnLoad();
        }

        private void ResetSemiPersistentItems(On.GameManager.orig_ResetSemiPersistentItems orig, GameManager self)
        {
            orig(self);
            Ref.Settings.ResetSemiPersistentItems();
        }

        /*
         Scene Change event order
        - BeginSceneTransition = Modhooks.BeforeSceneLoad
        - IsReadyToActivate: fires over 100 times
        - NotifyFetchComplete
        - {long break}
        - SceneManager.sceneLoaded
        - SceneManager.activeSceneChanged
        - GameManager.BeginScene
        - {short break}
        - GameManager.OnNextLevelReady -> GameManager.EnterHero -> HeroController.EnterScene
        */

        private void OnNotifyFetchComplete(On.GameManager.SceneLoadInfo.orig_NotifyFetchComplete orig, GameManager.SceneLoadInfo self)
        {
            Scene target = UnityEngine.SceneManagement.SceneManager.GetSceneByName(self.SceneName);
            if (!target.IsValid())
            {
                orig(self);
                target = UnityEngine.SceneManagement.SceneManager.GetSceneByName(self.SceneName);
                if (!target.IsValid())
                {
                    LogWarn($"Unable to find scene {self.SceneName} in OnNotifyFetchComplete!");
                    return;
                }
                InvokeOnSceneFetched(target);
                return;
            }

            InvokeOnSceneFetched(target);
            orig(self);
        }

        private void InvokeOnSceneFetched(Scene target)
        {
            foreach (var placement in SET?.GetPlacements())
            {
                if (placement is null) continue;
                else
                {
                    try
                    {
                        placement.OnSceneFetched(target);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in OnSceneFetched for {placement?.Name ?? "Unknown Placement"}:\n{e}");
                    }
                }
            }
        }

        private void OnOnNextLevelReady(On.GameManager.orig_OnNextLevelReady orig, GameManager self)
        {
            Scene next = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            foreach (var placement in SET?.GetPlacements())
            {
                if (placement is null) continue;
                else
                {
                    try
                    {
                        placement.OnNextSceneReady(next);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in OnNextLevelReady for {placement?.Name ?? "Unknown Placement"}:\n{e}");
                    }
                }
            }

            orig(self);
        }

        private void OnActiveSceneChanged(Scene from, Scene to)
        {
            Ref.Settings.ResetPersistentItems();
            foreach (var placement in SET?.GetPlacements())
            {
                if (placement is null) continue;
                else
                {
                    try
                    {
                        placement.OnActiveSceneChanged(from, to);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in OnActiveSceneChanged for {placement?.Name ?? "Unknown Placement"}:\n{e}");
                    }
                }
            }
        }

        private void ApplyLocationFsmEdits(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);

            // component-based default container support
            switch (fsm.FsmName)
            {
                case "Shiny Control":
                    ShinyUtility.ModifyFsm(fsm);
                    break;
                case "Bottle Control": // not all bottle controls are grub bottles, but GrubJarUtility checks for the ContainerInfo component before doing anything
                    GrubJarUtility.ModifyFsm(fsm);
                    break;
                case "Geo Rock":
                    GeoRockUtility.ModifyFsm(fsm);
                    break;
                case "Chest Control":
                    ChestUtility.ModifyFsm(fsm);
                    break;
            }

            string sceneName = fsm.gameObject.scene.name;
            foreach (var loc in SET?.GetPlacements() ?? new AbstractPlacement[0])
            {
                if (loc is null || string.IsNullOrEmpty(loc.SceneName)) continue;
                if (SceneUtil.IsSubscene(sceneName, loc.SceneName))
                {
                    try
                    {
                        loc.OnEnableFsm(fsm);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in LocationFsmEdits for {loc?.Name ?? "Unknown Location"}:\n{e}");
                    }
                }
            }
        }

        private string OnLanguageGet(string convo, string sheet)
        {
            return SET?.GetPlacements().Select(p => SafeLanguageGet(p, convo, sheet)).FirstOrDefault(p => p != null) ?? Language.Language.GetInternal(convo, sheet);
        }

        private string SafeLanguageGet(AbstractPlacement p, string convo, string sheet)
        {
            try
            {
                return p?.OnLanguageGet(convo, sheet);
            }
            catch (Exception e)
            {
                LogError($"Error in OnLanguageGet for {p?.Name ?? "Unknown Placement"}:\n{e}");
                return null;
            }
        }

        public override string GetVersion()
        {
            return "1.0.1";
        }

        public override int LoadPriority() => -2;

        public override List<(string, string)> GetPreloadNames()
        {
            return ObjectCache.GetPreloadNames();
        }

        public static void ChangeStartGame(StartDef start)
        {
            if (receivedChangeStartRequest) return;
            receivedChangeStartRequest = true;
            
            StartDef.start = start;
            On.GameManager.StartNewGame += StartDef.OverrideStartNewGame;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartDef.CreateRespawnMarker;
            StartDef.HookBenchwarp();
        }
    }
    
    
}
