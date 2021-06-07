using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using HutongGames.PlayMaker;
using ItemChanger.Actions;
using ItemChanger.UIDefs;
using Modding;
using MonoMod;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItemChanger.Locations;
using ItemChanger.Placements;
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
            XmlManager.Load();
            LanguageStringManager.Load();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ObjectCache.Setup(preloadedObjects);
            //readyForChangeItems = true;
            MessageController.Setup();

            CustomSkillManager.Hook();
            On.PlayMakerFSM.OnEnable += ApplyLocationFsmEdits;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ApplyLocationSceneEdits;
            foreach (var loc in SET.GetLocations()) loc.OnHook();
        }

        private void ApplyLocationSceneEdits(Scene arg0, Scene arg1)
        {
            foreach (var loc in SET?.GetLocations() ?? new AbstractPlacement[0])
            {
                if (loc is null) continue;
                if (loc.SceneName == arg1.name)
                {
                    try
                    {
                        loc.OnActiveSceneChanged();
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in LocationSceneEdits for {loc?.name ?? "Unknown Location"}:\n{e}");
                    }
                }
            }
        }

        private void ApplyLocationFsmEdits(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            if (self.FsmName == "Surface Water Region")
            {
                FsmState splash = self.GetState("Big Splash?");
                FsmStateAction acidDeath = new FsmStateActions.RandomizerExecuteLambda(() =>
                {
                    HeroController.instance.TakeDamage(self.gameObject, CollisionSide.other, 1, (int)GlobalEnums.HazardType.ACID);
                    PlayMakerFSM.BroadcastEvent("SWIM DEATH");
                });

                splash.AddFirstAction(acidDeath);
                splash.AddTransition("SWIM DEATH", "Idle");
                foreach (var c in GameObject.Find("waterways_water_components").GetComponentsInChildren<SpriteRenderer>()) c.color = new Color(228f / 255f, 111f / 255f, 52f / 255f);
                foreach (var g in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (g.name.StartsWith("water_fog")) g.GetComponent<SpriteRenderer>().color = new Color(228f / 255f, 111f / 255f, 52 / 255f);
                }
            }

            foreach (var loc in SET?.GetLocations() ?? new AbstractPlacement[0])
            {
                if (loc is null) continue;
                if (loc.SceneName == self.gameObject.scene.name)
                {
                    try
                    {
                        loc.OnEnableFsm(self);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error in LocationFsmEdits for {loc?.name ?? "Unknown Location"}:\n{e}");
                    }
                }
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

        public static void Reset()
        {
            if (receivedChangeItemsRequest)
            {
                LanguageStringManager.Unhook();
                PDHooks.Unhook();
                ItemChangerSettings.Unhook();
                RandomizerAction.UnHook();
                RandomizerAction.ClearActions();
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= EditScene;
                receivedChangeItemsRequest = false;
            }
            if (receivedChangeStartRequest)
            {
                On.GameManager.StartNewGame -= StartLocation.OverrideStartNewGame;
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartLocation.CreateRespawnMarker;
                receivedChangeStartRequest = false;
                StartLocation.UnHookBenchwarp();
            }
        }

        public static void ChangeItems(List<(_Item, _Location)> ItemLocationPairs, ItemChangerSettings settings = null, Default.Shops.DefaultShopItems defaultShopItems = Default.Shops.DefaultShopItems.None)
        {
            if (receivedChangeItemsRequest) return;
            receivedChangeItemsRequest = true;
            LanguageStringManager.Load();
            PDHooks.Hook();
            ItemChangerSettings.Hook();
            RandomizerAction.Hook();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += EditScene;

            // carefully sort to make sure ids are correctly assigned, if the list is not changed
            ItemLocationPairs.Sort(
                (pair1, pair2) =>
                {
                    int result = string.Compare(pair1.Item1.name, pair2.Item1.name);
                    return result == 0 ? string.Compare(pair1.Item2.name, pair2.Item2.name) : result;
                });
            _ILP.Process(ItemLocationPairs);
            AdditiveManager.Setup();
            ItemChangerSettings.currentSettings = settings ?? new ItemChangerSettings();
            ItemChangerSettings.Update();
            RandomizerAction.CreateActions(_ILP.ILPs.Values, defaultShopItems);
        }

        public static void ChangeStartGame(StartLocation start)
        {
            if (receivedChangeStartRequest) return;
            receivedChangeStartRequest = true;
            
            StartLocation.start = start;
            On.GameManager.StartNewGame += StartLocation.OverrideStartNewGame;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartLocation.CreateRespawnMarker;
            StartLocation.HookBenchwarp();
        }

        private static void EditScene(Scene from, Scene to)
        {
            // this is required to reset save settings before loading a new file
            if (to.name == SceneNames.Menu_Title) instance.Settings = new SaveSettings();

            // this implements RandomizerActions of type GameObject
            if (GameManager.instance.IsGameplayScene())
            {
                try
                {
                    // In rare cases, this is called before the previous scene has unloaded
                    // Deleting old randomizer shinies to prevent issues
                    foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (g.name.Contains("Randomizer Shiny") || g.name.Contains("New Shiny"))
                        {
                            GameObject.DestroyImmediate(g);
                        }
                    }

                    RandomizerAction.EditShinies();
                }
                catch (Exception e)
                {
                    instance.LogError($"Error applying RandomizerActions to scene {to.name}:\n" + e);
                }
            }
        }
    }
    
    
}
