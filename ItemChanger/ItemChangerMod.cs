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
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItemChanger.Components;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Util;
using TMPro;
using ItemChanger.Internal;

namespace ItemChanger
{
    public class ItemChangerMod : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<Settings>
    {
        internal static ItemChangerMod instance;

        internal static Settings SET { get; private set; } = new Settings();
        internal static GlobalSettings GS { get; private set; } = new GlobalSettings();

        public ItemChangerMod()
        {
            if (instance != null) throw new NotSupportedException("Cannot construct multiple instances of ItemChangerMod.");

            instance = this;
            SpriteManager.LoadEmbeddedPngs("ItemChanger.Resources.");
            LanguageStringManager.Load();
            Finder.Load();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ObjectCache.Setup(preloadedObjects);
            MessageController.Setup();

            On.GameManager.StartNewGame += BeforeStartNewGameHook;
            BeforeStartNewGame += OnStart;
            //ModHooks.Instance.NewGameHook += OnStart;
            ModHooks.SavegameLoadHook += OnLoad;
            On.GameManager.ResetSemiPersistentItems += ResetSemiPersistentItems;
            CustomSkillManager.Hook();
            DialogueCenter.Hook();
            WorldEventManager.Hook();
            ShopUtil.HookShops();
            On.PlayMakerFSM.OnEnable += ApplyLocationFsmEdits;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
            On.GameManager.OnNextLevelReady += OnOnNextLevelReady;
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += OnNotifyFetchComplete;
            ModHooks.LanguageGetHook += OnLanguageGet;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartDef.OnSceneChange;
            StartDef.HookBenchwarp();
        }

        private void OnStart()
        {
            //Tests.Tests.CostChestTest();
            foreach (var loc in SET.GetPlacements()) loc.OnLoad();
        }

        private void OnLoad(int id)
        {
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

        private void TitleReset()
        {
            foreach (var placement in SET?.GetPlacements())
            {
                placement?.OnUnload();
            }
            SET = new Settings();
        }

        private void OnActiveSceneChanged(Scene from, Scene to)
        {
            Ref.Settings.ResetPersistentItems();
            if (to.name == SceneNames.Menu_Title)
            {
                TitleReset();
            }

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

            // component-based default container support, called before and after placement hooks
            Container.OnEnable(fsm);

            string sceneName = fsm.gameObject.scene.name;
            foreach (var loc in SET?.GetPlacements() ?? new AbstractPlacement[0])
            {
                loc?.OnEnableGlobal(fsm);
                if (!string.IsNullOrEmpty(loc?.SceneName) && SceneUtil.IsSubscene(sceneName, loc.SceneName))
                {
                    try
                    {
                        loc.OnEnableLocal(fsm);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in LocationFsmEdits for {loc?.Name ?? "Unknown Location"}:\n{e}");
                    }
                }
            }

            Container.OnEnable(fsm);
        }

        private string OnLanguageGet(string convo, string sheet, string orig)
        {
            LanguageGetArgs args = new LanguageGetArgs(convo, sheet, orig);
            foreach (AbstractPlacement p in SET?.GetPlacements())
            {
                try
                {
                    p.OnLanguageGet(args);
                }
                catch (Exception e)
                {
                    LogError($"Error in OnLanguageGet for {p?.Name ?? "Unknown Placement"}:\n{e}");
                }
            }
            return args.current;
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

        public static event Action BeforeStartNewGame;
        public static event Action AfterStartNewGame;
        private void BeforeStartNewGameHook(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            try
            {
                BeforeStartNewGame?.Invoke();
            }
            catch(Exception e)
            {
                LogError($"Error in BeforeStartNewGame event:\n{e}");
                throw;
            }

            if (StartDef.Start != null)
            {
                StartDef.OverrideStartNewGame(orig, self, permadeathMode, bossRushMode);
            }
            else
            {
                orig(self, permadeathMode, bossRushMode);
            }

            try
            {
                AfterStartNewGame?.Invoke();
            }
            catch (Exception e)
            {
                LogError($"Error in AfterStartNewGame event:\n{e}");
                throw;
            }
        }

        public static void ChangeStartGame(StartDef start)
        {
            //if (receivedChangeStartRequest) return;
            //receivedChangeStartRequest = true;
            
            SET.Start = start;
            //On.GameManager.StartNewGame += StartDef.OverrideStartNewGame;
            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartDef.CreateRespawnMarker;
            //StartDef.HookBenchwarp();
        }

        /// <summary>
        /// Adds placements to local settings, with handling for placements with the same name.
        /// </summary>
        /// <param name="placements">The placements to add to the local settings.</param>
        /// <param name="conflictResolution">The action if a placement already exists in settings with the same name.</param>
        public static void AddPlacements(IEnumerable<AbstractPlacement> placements, PlacementConflictResolution conflictResolution = PlacementConflictResolution.MergeKeepingNew)
        {
            bool inGame = GameManager.instance?.sceneName != "Menu_Title";
            foreach (var p in placements)
            {
                if (SET.Placements.TryGetValue(p.Name, out var existsP))
                {
                    switch (conflictResolution)
                    {
                        case PlacementConflictResolution.MergeKeepingNew:
                            p.Items.AddRange(existsP.Items);
                            SET.Placements[p.Name] = p;
                            break;
                        case PlacementConflictResolution.MergeKeepingOld:
                            existsP.Items.AddRange(p.Items);
                            break;
                        case PlacementConflictResolution.Replace:
                            SET.Placements[p.Name] = p;
                            break;
                        case PlacementConflictResolution.Ignore:
                            break;
                        case PlacementConflictResolution.Throw:
                            throw new ArgumentException($"A placement with name {p.Name} already exists.");
                    }
                }
                else SET.Placements.Add(p.Name, p);
                if (inGame && p == SET.Placements[p.Name]) p.OnLoad();
            }
        }

        public enum PlacementConflictResolution
        {
            /// <summary>
            /// Keep new placement, discard old placement, and append items of old placement to new placement.
            /// </summary>
            MergeKeepingNew,
            /// <summary>
            /// Keep old placement, discard new placement, and append items of new placement to old placement.
            /// </summary>
            MergeKeepingOld,
            /// <summary>
            /// Keep new placement, discard old placement
            /// </summary>
            Replace,
            /// <summary>
            /// Keep old placement, discard new placement
            /// </summary>
            Ignore,
            Throw
        }

        public void OnLoadLocal(Settings s)
        {
            SET = s;
        }

        public Settings OnSaveLocal()
        {
            return SET;
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return GS;
        }
    }
    
    
}
