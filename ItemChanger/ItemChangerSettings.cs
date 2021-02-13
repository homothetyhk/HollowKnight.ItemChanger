using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Modding.Logger;

namespace ItemChanger
{
    public class ItemChangerSettings
    {
        public static ItemChangerSettings currentSettings;

        public bool forceUseFirstSimpleKeyOnWaterways = true;
        public bool unlockAllColosseumTrials = true;
        public bool reusableCityCrestWithNoHardSave = true;
        public bool openLowerBeastsDenThroughShortcut = true;
        public bool removeBeastsDenHardSave = true;
        public bool skipDreamerTextBeforeDreamNail = true;
        public bool blockLegEaterDeath = true;
        public bool oneBlueHPLifebloodCoreDoor = true;
        public bool reduceBaldurHP = true;
        public bool transitionQOL = true;
        public bool miscSkipFixes = true;
        public bool moveSeerLeft = true;
        public bool fixVoidHeart = true;
        public bool blockZoteDeath = true;

        public bool startWithoutFocus = false;

        public bool colo1ItemPrompt = true;
        public bool colo2ItemPrompt = true;
        public bool flowerQuestPrompt = true;
        public bool whitePalacePrompt = true;

        public HashSet<string> skipExtraPlatformScenes;

        internal static void Hook()
        {
            currentSettings = new ItemChangerSettings();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += AfterSceneChange;
            On.HeroController.CanFocus += OverrideCanFocus;
            On.PlayMakerFSM.OnEnable += FixVoidHeart;
            TransitionHooks.Hook();
        }

        

        internal static void Unhook()
        {
            currentSettings = new ItemChangerSettings();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= AfterSceneChange;
            On.HeroController.CanFocus -= OverrideCanFocus;
            On.PlayMakerFSM.OnEnable -= FixVoidHeart;
            TransitionHooks.Unhook();
        }


        private static bool OverrideCanFocus(On.HeroController.orig_CanFocus orig, HeroController self)
        {
            if(currentSettings.startWithoutFocus && !ItemChanger.instance.Settings.canFocus)
            {
                return false;
            }
            return orig(self);
        }

        private static void FixVoidHeart(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            if (currentSettings.blockZoteDeath && self.FsmName == "Check Zote Death")
            {
                GameObject.Destroy(self);
                return;
            }

            if (currentSettings.fixVoidHeart)
            {
                if (self.FsmName == "Shade Control")
                {
                    if (self.gameObject?.name?.StartsWith("Hollow Shade") ?? false)
                    {
                        self.FsmVariables.FindFsmBool("Friendly").Value = false;
                        self.GetState("Pause").ClearTransitions();
                        self.GetState("Pause").AddTransition("FINISHED", "Init");
                    }
                }
                else if (self.FsmName == "Control")
                {
                    if (self.gameObject?.name?.StartsWith("Shade Sibling") ?? false)
                    {
                        self.FsmVariables.FindFsmBool("Friendly").Value = false;
                        self.GetState("Pause").ClearTransitions();
                        self.GetState("Pause").AddTransition("FINISHED", "Init");
                    }
                }
            }
        }

        internal static void Update()
        {
            currentSettings.UpdatePrompts();
        }



        internal static void AfterSceneChange(Scene from, Scene to)
        {
            if (Platform.Platforms.TryGetValue(to.name, out List<Platform> plats))
            {
                if (!currentSettings.skipExtraPlatformScenes?.Contains(to.name) ?? true)
                {
                    foreach (Platform plat in plats) plat.Deploy();
                }
            }

            switch (to.name)
            {
                case SceneNames.Crossroads_ShamanTemple when currentSettings.reduceBaldurHP:
                case SceneNames.Fungus1_28 when currentSettings.reduceBaldurHP:
                case SceneNames.Crossroads_11_alt when currentSettings.reduceBaldurHP:
                    foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (obj.name.Contains("Blocker"))
                        {
                            HealthManager hm = obj.GetComponent<HealthManager>();
                            if (hm != null)
                            {
                                hm.hp = 5;
                            }
                            PlayMakerFSM fsm = FSMUtility.LocateFSM(obj, "Blocker Control");
                            if (fsm != null)
                            {
                                fsm.GetState("Can Roller?").RemoveActionsOfType<IntCompare>();
                            }
                        }
                    }
                    break;

                case SceneNames.RestingGrounds_07 when currentSettings.moveSeerLeft:
                    GameObject.Find("Dream Moth").transform.Translate(new Vector3(-5f, 0f));
                    break;

                case SceneNames.Ruins2_04 when currentSettings.forceUseFirstSimpleKeyOnWaterways:
                    FsmState hotSpringsKey = GameObject.Find("Inspect").LocateMyFSM("Conversation Control").GetState("Got Key?");
                    hotSpringsKey.RemoveActionsOfType<IntCompare>();
                    hotSpringsKey.AddAction(new RandomizerExecuteLambda(() =>
                    {
                        if (GameManager.instance.GetPlayerDataInt("simpleKeys") > 1 || (PlayerData.instance.openedWaterwaysManhole && GameManager.instance.GetPlayerDataInt("simpleKeys") > 0)) PlayMakerFSM.BroadcastEvent("YES");
                        else PlayMakerFSM.BroadcastEvent("NO");
                    }));
                    break;

                case SceneNames.Town when currentSettings.forceUseFirstSimpleKeyOnWaterways:
                    FsmState jijiKey = GameObject.Find("Jiji Door").LocateMyFSM("Conversation Control").GetState("Key?");
                    jijiKey.RemoveActionsOfType<GetPlayerDataInt>();
                    jijiKey.RemoveActionsOfType<IntCompare>();
                    jijiKey.AddAction(new RandomizerExecuteLambda(() =>
                    {
                        if (GameManager.instance.GetPlayerDataInt("simpleKeys") > 1 || (PlayerData.instance.openedWaterwaysManhole && GameManager.instance.GetPlayerDataInt("simpleKeys") > 0)) PlayMakerFSM.BroadcastEvent("KEY");
                        else PlayMakerFSM.BroadcastEvent("NOKEY");
                    }));
                    break;

                case SceneNames.Room_Colosseum_01 when currentSettings.unlockAllColosseumTrials:
                    PlayerData.instance.colosseumBronzeOpened = true;
                    PlayerData.instance.colosseumSilverOpened = true;
                    PlayerData.instance.colosseumGoldOpened = true;
                    GameObject.Find("Silver Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").ClearTransitions();
                    GameObject.Find("Silver Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").AddTransition("FINISHED", "Box Up YN");
                    GameObject.Find("Gold Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").ClearTransitions();
                    GameObject.Find("Gold Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").AddTransition("FINISHED", "Box Up YN");
                    break;

                case SceneNames.Fungus2_21 when currentSettings.reusableCityCrestWithNoHardSave:
                    FSMUtility.LocateFSM(GameObject.Find("City Gate Control"), "Conversation Control")
                        .GetState("Activate").RemoveActionsOfType<SetPlayerDataBool>();

                    FsmState gateSlam = FSMUtility.LocateFSM(GameObject.Find("Ruins_gate_main"), "Open")
                        .GetState("Slam");
                    gateSlam.RemoveActionsOfType<SetPlayerDataBool>();
                    gateSlam.RemoveActionsOfType<CallMethodProper>();
                    gateSlam.RemoveActionsOfType<SendMessage>();
                    break;

                case SceneNames.Deepnest_Spider_Town:
                    if (currentSettings.openLowerBeastsDenThroughShortcut)
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_Spider_Town",
                            id = "Collapser Small (12)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    if (currentSettings.removeBeastsDenHardSave)
                    {
                        FsmState denHardSave = GameObject.Find("RestBench Spider").LocateMyFSM("Fade").GetState("Land");
                        denHardSave.RemoveActionsOfType<CallMethodProper>();
                        denHardSave.RemoveActionsOfType<SendMessage>();
                        denHardSave.RemoveActionsOfType<SetPlayerDataBool>();
                    }
                    break;

                case SceneNames.RestingGrounds_04 when currentSettings.skipDreamerTextBeforeDreamNail:
                    FsmState dreamerPlaqueInspect = FSMUtility
                        .LocateFSM(GameObject.Find("Dreamer Plaque Inspect"), "Conversation Control")
                        .GetState("Hero Anim");
                    dreamerPlaqueInspect.RemoveActionsOfType<ActivateGameObject>();
                    dreamerPlaqueInspect.RemoveTransitionsTo("Fade Up");
                    dreamerPlaqueInspect.AddTransition("FINISHED", "Map Msg?");

                    PlayMakerFSM dreamerScene2 = FSMUtility.LocateFSM(GameObject.Find("Dreamer Scene 2"), "Control");
                    dreamerScene2.GetState("Take Control").RemoveTransitionsTo("Blast");
                    dreamerScene2.GetState("Take Control").AddTransition("FINISHED", "Fade Out");
                    dreamerScene2.GetState("Fade Out").RemoveTransitionsTo("Dial Wait");
                    dreamerScene2.GetState("Fade Out").AddTransition("FINISHED", "Set Compass Point");
                    break;

                case SceneNames.Fungus2_26 when currentSettings.blockLegEaterDeath:
                    PlayMakerFSM legEater = FSMUtility.LocateFSM(GameObject.Find("Leg Eater"), "Conversation Control");
                    FsmState legEaterChoice = legEater.GetState("Convo Choice");
                    legEaterChoice.RemoveTransitionsTo("Convo 1");
                    legEaterChoice.RemoveTransitionsTo("Convo 2");
                    legEaterChoice.RemoveTransitionsTo("Convo 3");
                    legEaterChoice.RemoveTransitionsTo("Infected Crossroad");
                    legEaterChoice.RemoveTransitionsTo("Bought Charm");
                    legEaterChoice.RemoveTransitionsTo("Gold Convo");
                    legEaterChoice.RemoveTransitionsTo("All Gold");
                    legEaterChoice.RemoveTransitionsTo("Ready To Leave");
                    legEater.GetState("All Gold?").RemoveTransitionsTo("No Shop");
                    PlayerData.instance.legEaterLeft = false;
                    break;

                case SceneNames.Abyss_06_Core when currentSettings.oneBlueHPLifebloodCoreDoor:
                    if (PlayerData.instance.healthBlue > 0 || PlayerData.instance.joniHealthBlue > 0 || GameManager.instance.entryGateName == "left1")
                    {
                        PlayerData.instance.SetBoolInternal("blueVineDoor", true);
                        PlayMakerFSM BlueDoorFSM = GameObject.Find("Blue Door").LocateMyFSM("Control");
                        BlueDoorFSM.GetState("Init").RemoveTransitionsTo("Got Charm");
                    }
                    break;

                // Make Cliffs shade skip feasible
                case SceneNames.Fungus1_28 when currentSettings.miscSkipFixes:
                    GameObject cliffsCrawlid = GameObject.Instantiate(GameObject.Find("Crawler"));
                    cliffsCrawlid.SetActive(true);
                    cliffsCrawlid.transform.position = new Vector2(74f, 31f);
                    foreach (GameObject g in GameManager.FindObjectsOfType<GameObject>())
                    {
                        if (g.transform.GetPositionX() < 75 && g.transform.GetPositionX() > 70 && g.transform.GetPositionY() < 54 && g.transform.GetPositionY() > 33)
                        {
                            GameObject.Destroy(g);
                        }
                    }
                    break;

                // Make descending dark spikes pogoable
                case SceneNames.Mines_35 when currentSettings.miscSkipFixes:
                    foreach (NonBouncer nonBounce in GameObject.FindObjectsOfType<NonBouncer>())
                    {
                        if (nonBounce.gameObject.name.StartsWith("Spike Collider"))
                        {
                            nonBounce.active = false;
                            nonBounce.gameObject.AddComponent<RandomizerTinkEffect>();
                        }
                    }
                    break;

                // Wings-only pogo to Soul Sanctum
                case SceneNames.Ruins1_05 when currentSettings.miscSkipFixes:
                    GameObject chandelier = GameObject.Find("ruind_dressing_light_02 (10)");
                    chandelier.transform.SetPositionX(chandelier.transform.position.x - 2);
                    chandelier.GetComponent<NonBouncer>().active = false;
                    break;

                // Sign pogo to get past right-side city without items
                case SceneNames.Ruins2_04 when currentSettings.miscSkipFixes:
                    {
                        GameObject plat = ObjectCache.SmallPlatform;
                        plat.SetActive(true);
                        plat.transform.position = new Vector2(18f, 10f);
                    }
                    break;

                // Respawn jars in Collector's room to allow wings only access
                case SceneNames.Ruins2_11 when currentSettings.miscSkipFixes:
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (1)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (2)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (3)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (4)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (5)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (6)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (7)",
                        activated = false,
                        semiPersistent = false
                    });
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (8)",
                        activated = false,
                        semiPersistent = false
                    });
                    break;

            }
        }

        internal void UpdatePrompts()
        {
            if (colo1ItemPrompt)
            {
                ILP ilp = ILP.ILPs.FirstOrDefault(kvp => kvp.Value.location.sceneName == SceneNames.Room_Colosseum_Bronze).Value;
                if (ilp == null)
                {
                    LogWarn("Requested Colo 1 hint, but did not supply location in Room_Colosseum_Bronze.");
                    colo1ItemPrompt = false;
                }
                else
                {
                    LanguageStringManager.SetString(
                            "Prompts",
                            "TRIAL_BOARD_BRONZE",
                            "Trial of the Warrior. Fight for " + Language.Language.Get(ilp.item.nameKey, "UI") + ".\n" + "Place a mark and begin the Trial?"
                                );
                }
            }
            else
            {
                LanguageStringManager.ResetString("Prompts", "TRIAL_BOARD_BRONZE");
            }

            if (colo2ItemPrompt)
            {
                ILP ilp = ILP.ILPs.FirstOrDefault(kvp => kvp.Value.location.sceneName == SceneNames.Room_Colosseum_Silver).Value;
                if (ilp == null)
                {
                    LogWarn("Requested Colo 2 hint, but did not supply location in Room_Colosseum_Silver.");
                    colo2ItemPrompt = false;
                }
                else
                {
                    LanguageStringManager.SetString(
                            "Prompts",
                            "TRIAL_BOARD_SILVER",
                            "Trial of the Conqueror. Fight for " + Language.Language.Get(ilp.item.nameKey, "UI") + ".\n" + "Place a mark and begin the Trial?"
                                );
                }
            }
            else
            {
                LanguageStringManager.ResetString("Prompts", "TRIAL_BOARD_SILVER");
            }


            if (flowerQuestPrompt)
            {
                ILP ilp = ILP.ILPs.FirstOrDefault(kvp => kvp.Value.location.sceneName == SceneNames.Room_Mansion).Value;
                if (ilp == null)
                {
                    LogWarn("Requested Flower Quest hint, but did not supply location in Room_Mansion.");
                    flowerQuestPrompt = false;
                }
                else
                {
                    LanguageStringManager.SetString(
                            "Prompts",
                            "XUN_OFFER",
                            "Accept the Gift, even knowing you'll only get a lousy " + Language.Language.Get(ilp.item.nameKey, "UI") + "?"
                                );
                }
            }
            else
            {
                LanguageStringManager.ResetString("Prompts", "XUN_OFFER");
            }


            if (whitePalacePrompt)
            {
                ILP ilp = ILP.ILPs.FirstOrDefault(kvp => kvp.Value.location.sceneName == SceneNames.White_Palace_09).Value;
                if (ilp == null)
                {
                    LogWarn("Requested King Fragment hint, but did not supply location in White_Palace_09.");
                    whitePalacePrompt = false;
                }
                else
                {
                    LanguageStringManager.SetString(
                            "Lore Tablets",
                            "DUSK_KNIGHT_CORPSE",
                            "A corpse in white armour. You can clearly see the "
                                + Language.Language.Get(ilp.item.nameKey, "UI") + " it's holding, " +
                                "but for some reason you get the feeling you're going to have to go" +
                                " through an unnecessarily long gauntlet of spikes and sawblades just to pick it up."
                                );
                }
            }
            else
            {
                LanguageStringManager.ResetString("Lore Tablets", "DUSK_KNIGHT_CORPSE");
            }
        }
    }
}
