using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using HutongGames.PlayMaker;
using ItemChanger.Actions;
using Modding;
using MonoMod;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public class ItemChanger : Mod
    {
        internal static ItemChanger instance;
        internal static bool readyForChangeItems = false;
        internal static bool receivedChangeItemsRequest = false;
        internal static bool receivedChangeStartRequest = false;

        public SaveSettings Settings { get; set; } = new SaveSettings();
        public override ModSettings SaveSettings
        {
            get => Settings = Settings ?? new SaveSettings();
            set => Settings = value is SaveSettings saveSettings ? saveSettings : Settings;
        }

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
            readyForChangeItems = true;
        }

        public override string GetVersion()
        {
            return "1.0.1";
        }

        public override int LoadPriority() => -2;

        public override List<(string, string)> GetPreloadNames()
        {
            return ObjectCache.preloads;
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

        public static void ChangeItems(List<(Item, Location)> ItemLocationPairs, ItemChangerSettings settings = null, Default.Shops.DefaultShopItems defaultShopItems = Default.Shops.DefaultShopItems.None)
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
            ILP.Process(ItemLocationPairs);
            AdditiveManager.Setup();
            ItemChangerSettings.currentSettings = settings ?? new ItemChangerSettings();
            ItemChangerSettings.Update();
            RandomizerAction.CreateActions(ILP.ILPs.Values, defaultShopItems);
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
