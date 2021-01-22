using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using ItemChanger.FsmStateActions;
using SeanprCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public static class TransitionHooks
    {
        public static void Hook()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ApplyTransitionFixes;
            On.GameManager.BeginSceneTransition += OverrideBeginSceneTransition;
        }

        public static void Unhook() 
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ApplyTransitionFixes;
            On.GameManager.BeginSceneTransition += OverrideBeginSceneTransition;
        }

        private static void OverrideBeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            ApplySaveDataChanges(info.SceneName, info.EntryGateName);
            orig(self, info);
        }

        private static void ApplyTransitionFixes(Scene oldScene, Scene newScene)
        {
            if (!ItemChangerSettings.currentSettings.transitionQOL) return;

            switch (newScene.name)
            {
                case SceneNames.Abyss_06_Core:
                    // Opens floor to void heart (bypasses subsequent checks for kingsoul to be equipped)
                    if (GameManager.instance.entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedBlackEggPath", true);
                    }
                    if (PlayerData.instance.openedBlackEggPath)
                    {
                        GameObject.Destroy(GameObject.Find("floor_closed"));
                    }
                    break;
                case SceneNames.Deepnest_41:
                    if (GameManager.instance.entryGateName.StartsWith("left1"))
                    {
                        foreach (Transform t in GameObject.Find("Collapser Small (2)").FindGameObjectInChildren("floor1").transform)
                        {
                            if (t.gameObject.name.StartsWith("msk")) GameObject.Destroy(t.gameObject);
                        }
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (GameManager.instance.entryGateName.StartsWith("bot2"))
                    {
                        GameObject.Destroy(GameObject.Find("Quake Floor").FindGameObjectInChildren("Active").FindGameObjectInChildren("msk_generic"));
                        GameObject.Destroy(GameObject.Find("Quake Floor").FindGameObjectInChildren("Active").FindGameObjectInChildren("msk_generic (1)"));
                        GameObject.Destroy(GameObject.Find("Quake Floor").FindGameObjectInChildren("Active").FindGameObjectInChildren("msk_generic (2)"));
                        GameObject.Destroy(GameObject.Find("Quake Floor").FindGameObjectInChildren("Active").FindGameObjectInChildren("msk_generic (3)"));
                    }
                    break;
                case SceneNames.Fungus2_15:
                    if (GameManager.instance.entryGateName.StartsWith("left"))
                    {
                        GameObject.Destroy(GameObject.Find("deepnest_mantis_gate").FindGameObjectInChildren("Collider"));
                        GameObject.Destroy(GameObject.Find("deepnest_mantis_gate"));
                    }
                    break;
                case SceneNames.Fungus2_25:
                    if (GameManager.instance.entryGateName.StartsWith("right"))
                    {
                        GameObject.Destroy(GameObject.Find("mantis_big_door"));
                    }
                    break;
                case SceneNames.Ruins1_09:
                    if (GameManager.instance.entryGateName.StartsWith("t"))
                    {
                        GameObject.Destroy(GameObject.Find("Battle Gate"));
                        GameObject.Destroy(GameObject.Find("Battle Scene"));
                    }
                    break;

                // old randomizer stuff, hopefully shouldn't cause issues 
                case SceneNames.Ruins1_24:
                    // Stop the weird invisible floor from appearing if dive has been obtained
                    if (Ref.PD.quakeLevel > 0)
                    {
                        GameObject.Destroy(GameObject.Find("Roof Collider Battle"));
                    }

                    // Change battle gate to be destroyed if Soul Master is dead instead of it the player has quake
                    FsmState checkQuake = FSMUtility.LocateFSM(GameObject.Find("Battle Gate (1)"), "Destroy if Quake").GetState("Check");
                    checkQuake.RemoveActionsOfType<FsmStateAction>();
                    checkQuake.AddAction(new RandomizerBoolTest(nameof(PlayerData.killedMageLord), null, "DESTROY", true));
                    break;

                case SceneNames.Waterways_04:
                    if (GameManager.instance.entryGateName.StartsWith("b"))
                    {
                        GameObject[] gs = GameObject.FindObjectsOfType<GameObject>();
                        foreach (GameObject g in gs)
                        {
                            if (g.name.StartsWith("Mask"))
                            {
                                g.SetActive(false);
                            }
                        }
                    }
                    break;
                case SceneNames.White_Palace_03_hub:
                    {
                        GameObject[] gs = GameObject.FindObjectsOfType<GameObject>();
                        foreach (GameObject g in gs)
                        {
                            if (g.name.StartsWith("Progress"))
                            {
                                GameObject.Destroy(g);
                            }
                        }
                    }
                    break;
                case SceneNames.White_Palace_06:
                    if (GameObject.Find("Path of Pain Blocker") != null)
                    {
                        GameObject.Destroy(GameObject.Find("Path of Pain Blocker"));
                    }
                    break;

                // traverse first room of PoP backwards. Doesn't really belong here, but w/e
                case SceneNames.White_Palace_18:
                    const float SAW = 1.362954f;
                    GameObject saw = GameObject.Find("saw_collection/wp_saw (4)");

                    GameObject topSaw = GameObject.Instantiate(saw);
                    topSaw.transform.SetPositionX(165f);
                    topSaw.transform.SetPositionY(30.5f);
                    topSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

                    GameObject botSaw = GameObject.Instantiate(saw);
                    botSaw.transform.SetPositionX(161.4f);
                    botSaw.transform.SetPositionY(21.4f);
                    botSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);
                    break;
            }
        }

        // save data changes for entering transitions from unintended directions
        private static void ApplySaveDataChanges(string sceneName, string entryGateName)
        {
            if (!ItemChangerSettings.currentSettings.transitionQOL) return;
            if (string.IsNullOrEmpty(sceneName)) return;
            entryGateName = entryGateName ?? string.Empty;

            switch (sceneName)
            {
                case SceneNames.Tutorial_01:
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Tutorial_01",
                        id = "Initial Fall Impact",
                        activated = true,
                        semiPersistent = false
                    });
                    if (entryGateName.StartsWith("right"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Door",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Collapser Tute 01",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 1",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 2",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 3",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 4",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 5",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 7",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                case SceneNames.Abyss_01:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("dungDefenderWallBroken", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Abyss_03_c:
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Mask 1",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Mask 1 (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Abyss_05:
                    if (entryGateName == "right1")
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Abyss_05",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Cliffs_01:
                    if (entryGateName.StartsWith("right4"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Cliffs_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Cliffs_01",
                            id = "Breakable Wall grimm",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_04:
                    Ref.PD.menderState = 2;
                    Ref.PD.menderDoorOpened = true;
                    Ref.PD.hasMenderKey = true;
                    Ref.PD.menderSignBroken = true;
                    if (entryGateName.StartsWith("d"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_04",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_04",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_06:
                    // Opens gate in room after False Knight
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("shamanPillar", true);
                    }
                    break;
                case SceneNames.Crossroads_07:
                    if (entryGateName.StartsWith("left3"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_07",
                            id = "Tute Door 1",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_08:
                    if (entryGateName == "left2")
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_08",
                            id = "Battle Scene",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_09:
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_09",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                        PlayerData.instance.SetBool("crossroadsMawlekWall", true);
                    }
                    break;
                case SceneNames.Crossroads_21:
                    // Makes room visible entering from gwomb entrance
                    if (entryGateName.StartsWith("t"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Collapser Small",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_33:
                    if (entryGateName.StartsWith("left1"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_09",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                        PlayerData.instance.SetBool("crossroadsMawlekWall", true);
                    }
                    // Opens gate in room after False Knight
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("shamanPillar", true);
                    }
                    break;
                case SceneNames.Deepnest_01:
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus2_20",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_02:
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_02",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_03:
                    if (entryGateName.StartsWith("left2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_03",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_26:
                    if (entryGateName.StartsWith("left2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_26",
                            id = "Inverse Remasker",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_26",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_31:
                    if (entryGateName.StartsWith("right2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask (2)",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Breakable Wall (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("outskirtsWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_02",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_03:
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("outskirtsWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_02",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_16:
                    if (entryGateName.StartsWith("b"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_16",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus2_20:
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus2_20",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_02:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("oneWayArchive", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_47",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_13:
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("openedGardensStagStation", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_40",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_40:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("openedGardensStagStation", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_40",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_44:
                    GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "Fungus3_44",
                        id = "Secret Mask",
                        activated = true,
                        semiPersistent = false
                    });
                    break;
                case SceneNames.Fungus3_47:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("oneWayArchive", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_47",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Mines_05:
                    // breakable wall leading to Deep Focus
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("brokeMinersWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Mines_05",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_02:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedRestingGrounds02", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Resting Grounds Slide Floor",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_05:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("gladeDoorOpened", true);
                        PlayerData.instance.SetBool("dreamReward2", true);
                    }
                    if (entryGateName.StartsWith("b"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_05",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_06:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("openedRestingGrounds02", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Resting Grounds Slide Floor",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_10:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("restingGroundsCryptWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    if (entryGateName.StartsWith("top2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "Breakable Wall (5)",
                            activated = true,
                            semiPersistent = false
                        });
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "Breakable Wall (7)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Room_Town_Stag_Station:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("openedTownBuilding", true);
                        PlayerData.instance.SetBool("openedTown", true);
                    }
                    break;
                case SceneNames.Ruins1_05b:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedWaterwaysManhole", true);
                    }
                    break;
                case SceneNames.Ruins1_23:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("brokenMageWindow", true);
                        PlayerData.instance.SetBool("brokenMageWindowGlass", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_30",
                            id = "Quake Floor Glass (2)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_24:
                    if (entryGateName.StartsWith("right2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_24",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_30:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("brokenMageWindow", true);
                        PlayerData.instance.SetBool("brokenMageWindowGlass", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_30",
                            id = "Quake Floor Glass (2)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_31:
                    if (entryGateName.StartsWith("left3"))
                    {
                        PlayerData.instance.SetBool("openedMageDoor_v2", true);
                    }
                    if (entryGateName.StartsWith("left2"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_31",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case "Ruins1_31b":
                    if (entryGateName.StartsWith("right1"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_31",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_01:
                    if (entryGateName.StartsWith("t"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins2_01",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_04:
                    if (entryGateName.StartsWith("door_Ruin_House_03"))
                    {
                        PlayerData.instance.SetBool("city2_sewerDoor", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins_House_03",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    else if (entryGateName.StartsWith("door_Ruin_Elevator"))
                    {
                        PlayerData.instance.SetBool("bathHouseOpened", true);
                    }
                    break;
                case SceneNames.Ruins2_10:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("restingGroundsCryptWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_10b:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("bathHouseWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins_Bathhouse",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case "Ruins2_11_b":
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("openedLoveDoor", true);
                    }
                    break;
                case SceneNames.Ruins_House_03:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("city2_sewerDoor", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins_House_03",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins_Bathhouse:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("bathHouseWall", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Ruins_Bathhouse",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Town:
                    switch (entryGateName)
                    {
                        case "door_sly":
                            PlayerData.instance.SetBool("slyRescued", true);
                            PlayerData.instance.SetBool("openedSlyShop", true);
                            break;
                        case "door_station":
                            PlayerData.instance.SetBool("openedTownBuilding", true);
                            PlayerData.instance.SetBool("openedTown", true);
                            break;
                        case "door_mapper":
                            PlayerData.instance.SetBool("openedMapperShop", true);
                            break;
                        case "door_bretta":
                            PlayerData.instance.SetBool("brettaRescued", true);
                            break;
                        case "door_jiji":
                            PlayerData.instance.SetBool("jijiDoorUnlocked", true);
                            break;
                    }
                    if (entryGateName != "left1")
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Town",
                            id = "Door Destroyer",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_01:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("openedWaterwaysManhole", true);
                    }
                    if (entryGateName.StartsWith("r"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_01",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_02:
                    if (entryGateName.StartsWith("bot1"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_02",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_04:
                    if (entryGateName.StartsWith("b"))
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_04",
                            id = "Quake Floor (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_05:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("dungDefenderWallBroken", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_07:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("waterwaysAcidDrained", true);
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "Waterways_Crank_Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_08:
                    if (entryGateName == "left2")
                    {
                        GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                        {
                            sceneName = "Waterways_08",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_09:
                    if (entryGateName.StartsWith("left"))
                    {
                        PlayerData.instance.SetBool("waterwaysGate", true);
                    }
                    break;
                case SceneNames.White_Palace_13:
                    PlayerData.instance.SetBool(nameof(PlayerData.whitePalaceSecretRoomVisited), true);
                    break;
            }
        }

    }
}
