using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using SD = ItemChanger.Util.SceneDataUtil;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which applies a large number of fixes to allow entering rooms from unintended directions.
    /// </summary>
    [DefaultModule]
    public class TransitionFixes : Module
    {
        public override void Initialize()
        {
            Events.OnBeginSceneTransition += OnBeginSceneTransition;
            Events.OnSceneChange += OnSceneChange;
            Events.AddFsmEdit(SceneNames.Abyss_06_Core, new("Blue Door", "Control"), FixReverseBlueDoor);
            Events.AddFsmEdit(SceneNames.Abyss_06_Core, new("floor_closed", "Disappear"), FixReverseBirthplace);
        }

        public override void Unload()
        {
            Events.OnBeginSceneTransition -= OnBeginSceneTransition;
            Events.OnSceneChange -= OnSceneChange;
            Events.RemoveFsmEdit(SceneNames.Abyss_06_Core, new("Blue Door", "Control"), FixReverseBlueDoor);
            Events.RemoveFsmEdit(SceneNames.Abyss_06_Core, new("floor_closed", "Disappear"), FixReverseBirthplace);
        }

        /// <summary>
        /// Fixes targeted at Transitions in this collection will be ignored.
        /// </summary>
        public HashSet<Transition> ExcludedTransitionFixes = new();

        private void OnBeginSceneTransition(Transition t)
        {
            if (ExcludedTransitionFixes.Contains(t)) return;

            switch (t.SceneName)
            {
                case SceneNames.Tutorial_01:
                    SD.Save("Tutorial_01", "Initial Fall Impact");
                    if (t.GateName == "right1")
                    {
                        SD.Save("Tutorial_01", "Door");
                        SD.Save("Tutorial_01", "Collapser Tute 01");
                        SD.Save("Tutorial_01", "Tute Door 1");
                        SD.Save("Tutorial_01", "Tute Door 2");
                        SD.Save("Tutorial_01", "Tute Door 3");
                        SD.Save("Tutorial_01", "Tute Door 4");
                        SD.Save("Tutorial_01", "Tute Door 5");
                        SD.Save("Tutorial_01", "Tute Door 7");
                        SD.Save("Tutorial_01", "Break Floor 1");
                    }
                    break;

                case SceneNames.Abyss_01:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.dungDefenderWallBroken), true);
                        SD.Save("Waterways_05", "One Way Wall");
                    }
                    break;
                case SceneNames.Abyss_03_c:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Abyss_03_c", "Breakable Wall");
                        SD.Save("Abyss_03_c", "Mask 1");
                        SD.Save("Abyss_03_c", "Mask 1 (1)");
                    }
                    break;
                case SceneNames.Abyss_05:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Abyss_05", "Breakable Wall");
                    }
                    break;
                case SceneNames.Cliffs_01:
                    if (t.GateName == "right4")
                    {
                        SD.Save("Cliffs_01", "Breakable Wall");
                        SD.Save("Cliffs_01", "Breakable Wall grimm");
                    }
                    break;
                case SceneNames.Crossroads_04:
                    if (t.GateName.StartsWith("d"))
                    {
                        SD.Save("Crossroads_04", "Secret Mask");
                        SD.Save("Crossroads_04", "Secret Mask (1)");
                    }
                    break;
                case SceneNames.Crossroads_06:
                    // Opens gate in room after False Knight
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.shamanPillar), true);
                        SD.Save("Crossroads_06", "Raising Pillar");
                        SD.Save("Crossroads_06", "Gate Switch");
                    }
                    break;
                case SceneNames.Crossroads_07:
                    if (t.GateName == "left3")
                    {
                        SD.Save("Crossroads_07", "Tute Door 1");
                    }
                    break;
                case SceneNames.Crossroads_08:
                    if (t.GateName == "left2")
                    {
                        SD.Save("Crossroads_08", "Battle Scene");
                    }
                    break;
                case SceneNames.Crossroads_09:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Crossroads_09", "Break Floor 1");
                        PlayerData.instance.SetBool(nameof(PlayerData.crossroadsMawlekWall), true);
                    }
                    break;
                case SceneNames.Crossroads_21:
                    // Makes room visible entering from gwomb entrance
                    if (t.GateName == "top1")
                    {
                        SD.Save("Crossroads_21", "Breakable Wall");
                        SD.Save("Crossroads_21", "Collapser Small");
                        SD.Save("Crossroads_21", "Secret Mask (1)");
                    }
                    break;
                case SceneNames.Crossroads_33:
                    if (t.GateName == "left1")
                    {
                        SD.Save("Crossroads_09", "Break Floor 1");
                        PlayerData.instance.SetBool(nameof(PlayerData.crossroadsMawlekWall), true);
                    }
                    // Opens gate in room after False Knight
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.shamanPillar), true);
                        SD.Save("Crossroads_06", "Raising Pillar");
                        SD.Save("Crossroads_06", "Gate Switch");
                    }
                    break;
                case SceneNames.Deepnest_01:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Deepnest_01", "Breakable Wall");
                        SD.Save("Fungus2_20", "Breakable Wall Waterways");
                    }
                    break;
                case SceneNames.Deepnest_02:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Deepnest_02", "Breakable Wall");
                    }
                    break;
                case SceneNames.Deepnest_03:
                    if (t.GateName == "left2")
                    {
                        SD.Save("Deepnest_03", "Breakable Wall");
                    }
                    break;
                case SceneNames.Deepnest_26:
                    if (t.GateName == "left2")
                    {
                        SD.Save("Deepnest_26", "Inverse Remasker");
                        SD.Save("Deepnest_26", "Secret Mask (1)");
                    }
                    break;
                case SceneNames.Deepnest_31:
                    if (t.GateName == "right2")
                    {
                        SD.Save("Deepnest_31", "Secret Mask");
                        SD.Save("Deepnest_31", "Secret Mask (1)");
                        SD.Save("Deepnest_31", "Secret Mask (2)");
                        SD.Save("Deepnest_31", "Breakable Wall");
                        SD.Save("Deepnest_31", "Breakable Wall (1)");
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.outskirtsWall), true);
                        SD.Save("Deepnest_East_02", "One Way Wall");
                    }
                    break;
                case SceneNames.Deepnest_East_03:
                    if (t.GateName == "left2")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.outskirtsWall), true);
                        SD.Save("Deepnest_East_02", "One Way Wall");
                    }
                    // When entering from one of the other entrances, it's possible that the player will reach Cornifer before the big title popup
                    // appears; this can lead to a hard lock if the player interacts with Cornifer and then the popup appears during the interaction.
                    if (!t.GateName.StartsWith("left"))
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.visitedOutskirts), true);
                    }
                    break;
                case SceneNames.Deepnest_East_16:
                    if (t.GateName == "bot1")
                    {
                        SD.Save("Deepnest_East_16", "Quake Floor");
                    }
                    break;
                case SceneNames.Fungus2_20:
                    if (t.GateName == "left1")
                    {
                        SD.Save("Deepnest_01", "Breakable Wall");
                        SD.Save("Fungus2_20", "Breakable Wall Waterways");
                    }
                    break;
                case SceneNames.Fungus3_02:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.oneWayArchive), true);
                        SD.Save("Fungus3_47", "One Way Wall");
                    }
                    break;
                case SceneNames.Fungus3_13:
                    if (t.GateName == "left2")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedGardensStagStation), true);
                        SD.Save("Fungus3_40", "Gate Switch");
                    }
                    break;
                case SceneNames.Fungus3_40:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedGardensStagStation), true);
                        SD.Save("Fungus3_40", "Gate Switch");
                    }
                    break;
                case SceneNames.Fungus3_44:
                    if (t.GateName == "door1")
                    {
                        SD.Save("Fungus3_44", "Secret Mask");
                    }
                    break;
                case SceneNames.Fungus3_47:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.oneWayArchive), true);
                        SD.Save("Fungus3_47", "One Way Wall");
                    }
                    break;
                case SceneNames.Mines_05:
                    // breakable wall leading to Deep Focus
                    if (t.GateName == "left2")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.brokeMinersWall), true);
                        SD.Save("Mines_05", "Breakable Wall");
                    }
                    break;
                case SceneNames.RestingGrounds_02:
                    if (t.GateName == "bot1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedRestingGrounds02), true);
                        SD.Save("RestingGrounds_06", "Resting Grounds Slide Floor");
                        SD.Save("RestingGrounds_06", "Gate Switch");
                    }
                    break;
                case SceneNames.RestingGrounds_05:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.gladeDoorOpened), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.dreamReward2), true);
                    }
                    else if (t.GateName == "bot1")
                    {
                        SD.Save("RestingGrounds_05", "Quake Floor");
                    }
                    break;
                case SceneNames.RestingGrounds_06:
                    if (t.GateName == "top1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedRestingGrounds02), true);
                        SD.Save("RestingGrounds_06", "Resting Grounds Slide Floor");
                        SD.Save("RestingGrounds_06", "Gate Switch");
                    }
                    break;
                case SceneNames.RestingGrounds_10:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.restingGroundsCryptWall), true);
                        SD.Save("RestingGrounds_10", "One Way Wall");
                    }
                    else if (t.GateName == "top2")
                    {
                        SD.Save("RestingGrounds_10", "Breakable Wall (5)");
                        SD.Save("RestingGrounds_10", "Breakable Wall (7)");
                    }
                    break;
                case SceneNames.Room_Town_Stag_Station:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedTownBuilding), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.openedTown), true);
                    }
                    break;
                case SceneNames.Ruins1_05b:
                    if (t.GateName == "bot1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedWaterwaysManhole), true);
                    }
                    break;
                case SceneNames.Ruins1_23:
                    if (t.GateName == "top1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.brokenMageWindow), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.brokenMageWindowGlass), true);
                        SD.Save("Ruins1_30", "Quake Floor Glass (2)");
                    }
                    break;
                case SceneNames.Ruins1_24:
                    if (t.GateName == "right2")
                    {
                        SD.Save("Ruins1_24", "Secret Mask (1)");
                    }
                    break;
                case SceneNames.Ruins1_30:
                    if (t.GateName == "bot1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.brokenMageWindow), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.brokenMageWindowGlass), true);
                        SD.Save("Ruins1_30", "Quake Floor Glass (2)");
                    }
                    break;
                case SceneNames.Ruins1_31:
                    if (t.GateName == "left3")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedMageDoor_v2), true);
                    }
                    if (t.GateName == "left2")
                    {
                        SD.Save("Ruins1_31", "Ruins Lever");
                    }
                    break;
                case SceneNames.Ruins1_31b:
                    if (t.GateName == "right1")
                    {
                        SD.Save("Ruins1_31", "Ruins Lever");
                    }
                    break;
                case SceneNames.Ruins2_01:
                    if (t.GateName == "top1")
                    {
                        SD.Save("Ruins2_01", "Secret Mask");
                    }
                    break;
                case SceneNames.Ruins2_04:
                    if (t.GateName == "door_Ruin_House_03")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.city2_sewerDoor), true);
                        SD.Save("Ruins_House_03", "Ruins Lever");
                    }
                    else if (t.GateName == "door_Ruin_Elevator")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.bathHouseOpened), true);
                    }
                    break;
                case SceneNames.Ruins2_10:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.restingGroundsCryptWall), true);
                        SD.Save("RestingGrounds_10", "One Way Wall");
                    }
                    break;
                case SceneNames.Ruins2_10b:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.bathHouseWall), true);
                        SD.Save("Ruins_Bathhouse", "Breakable Wall");
                    }
                    break;
                case SceneNames.Ruins2_11_b:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedLoveDoor), true);
                    }
                    break;
                case SceneNames.Ruins_House_03:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.city2_sewerDoor), true);
                        SD.Save("Ruins_House_03", "Ruins Lever");
                    }
                    break;
                case SceneNames.Ruins_Bathhouse:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.bathHouseWall), true);
                        SD.Save("Ruins_Bathhouse", "Breakable Wall");
                    }
                    break;
                case SceneNames.Town:
                    switch (t.GateName)
                    {
                        case "door_sly":
                            PlayerData.instance.SetBool(nameof(PlayerData.slyRescued), true);
                            PlayerData.instance.SetBool(nameof(PlayerData.openedSlyShop), true);
                            break;
                        case "door_station":
                            PlayerData.instance.SetBool(nameof(PlayerData.openedTownBuilding), true);
                            PlayerData.instance.SetBool(nameof(PlayerData.openedTown), true);
                            break;
                        case "door_mapper":
                            PlayerData.instance.SetBool(nameof(PlayerData.openedMapperShop), true);
                            break;
                        case "door_bretta":
                            PlayerData.instance.SetBool(nameof(PlayerData.brettaRescued), true);
                            break;
                        case "door_jiji":
                            PlayerData.instance.SetBool(nameof(PlayerData.jijiDoorUnlocked), true);
                            break;
                        case "room_grimm":
                            PlayerData.instance.SetBool(nameof(PlayerData.troupeInTown), true);
                            break;
                        case "room_divine":
                            PlayerData.instance.SetBool(nameof(PlayerData.divineInTown), true);
                            break;
                    }
                    if (t.GateName != "left1")
                    {
                        SD.Save("Town", "Door Destroyer");
                    }
                    break;
                case SceneNames.Waterways_01:
                    if (t.GateName == "top1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.openedWaterwaysManhole), true);
                    }
                    if (t.GateName == "right1")
                    {
                        SD.Save("Waterways_01", "Breakable Wall Waterways");
                    }
                    break;
                case SceneNames.Waterways_02:
                    if (t.GateName == "bot1")
                    {
                        SD.Save("Waterways_02", "Quake Floor");
                    }
                    break;
                case SceneNames.Waterways_04:
                    if (t.GateName == "bot1")
                    {
                        SD.Save("Waterways_04", "Quake Floor (1)");
                    }
                    break;
                case SceneNames.Waterways_05:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.dungDefenderWallBroken), true);
                        SD.Save("Waterways_05", "One Way Wall");
                    }
                    if (t.GateName == "bot2")
                    {
                        SD.Save("Waterways_05", "Quake Floor");
                    }
                    break;
                case SceneNames.Waterways_07:
                    if (t.GateName == "right1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.waterwaysAcidDrained), true);
                        SD.Save("Waterways_05", "Waterways_Crank_Lever");
                    }
                    break;
                case SceneNames.Waterways_08:
                    if (t.GateName == "left2")
                    {
                        SD.Save("Waterways_08", "Breakable Wall Waterways");
                    }
                    break;
                case SceneNames.Waterways_09:
                    if (t.GateName == "left1")
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.waterwaysGate), true);
                    }
                    break;
                case SceneNames.White_Palace_13:
                    PlayerData.instance.SetBool(nameof(PlayerData.whitePalaceSecretRoomVisited), true);
                    break;
            }
        }

        private void FixReverseBlueDoor(PlayMakerFSM fsm)
        {
            if ((GameManager.instance.entryGateName != "left1" && GameManager.instance.entryGateName != Transition.door_dreamReturn) 
                || ExcludedTransitionFixes.Contains(new(SceneNames.Abyss_06_Core, GameManager.instance.entryGateName))) return;

            FsmState init = fsm.GetState("Init");

            init.RemoveActionsOfType<PlayerDataBoolTest>();
            init.AddLastAction(new Lambda(() => fsm.SendEvent("OPENED")));
        }

        private void FixReverseBirthplace(PlayMakerFSM fsm)
        {
            if (GameManager.instance.entryGateName != "bot1" || ExcludedTransitionFixes.Contains(new(SceneNames.Abyss_06_Core, "bot1"))) return;
            PlayerData.instance.SetBool(nameof(PlayerData.openedBlackEggPath), true);

            // route the floor closed fsm to deactivate regardless of whether charm 36 is equipped
            fsm.GetState("State 1").Transitions[0].FsmEvent = FsmEvent.Finished;
        }


        private void OnSceneChange(Scene newScene)
        {
            Transition t = new(newScene.name, GameManager.instance.entryGateName);
            if (ExcludedTransitionFixes.Contains(t)) return;

            switch (t.SceneName)
            {
                case SceneNames.Crossroads_03:
                    if (t.GateName.StartsWith("bot1"))
                    {
                        RemoveInfectedBlockades.DestroyBlockade_Crossroads_03(newScene);
                    }
                    break;
                case SceneNames.Crossroads_06:
                    if (t.GateName.StartsWith("right1"))
                    {
                        RemoveInfectedBlockades.DestroyBlockade_Crossroads_06(newScene);
                    }
                    break;
                case SceneNames.Crossroads_10:
                    if (t.GateName.StartsWith("left1"))
                    {
                        RemoveInfectedBlockades.DestroyBlockade_Crossroads_10(newScene);
                    }
                    break;
                case SceneNames.Crossroads_19:
                    if (t.GateName.StartsWith("top1"))
                    {
                        RemoveInfectedBlockades.DestroyBlockade_Crossroads_19(newScene);
                    }
                    break;
                case SceneNames.Deepnest_41:
                    if (t.GateName.StartsWith("left1"))
                    {
                        foreach (Transform u in newScene.FindGameObject("Collapser Small (2)").transform.Find("floor1"))
                        {
                            if (u.name.StartsWith("msk")) UObject.Destroy(u.gameObject);
                        }
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (t.GateName.StartsWith("bot2"))
                    {
                        foreach (Transform u in newScene.FindGameObject("Quake Floor/Active").transform)
                        {
                            if (u.name.StartsWith("msk")) UObject.Destroy(u.gameObject);
                        }
                    }
                    break;
                case SceneNames.Fungus2_15:
                    if (t.GateName.StartsWith("left"))
                    {
                        UObject.Destroy(newScene.FindGameObject("deepnest_mantis_gate").FindChild("Collider"));
                        UObject.Destroy(newScene.FindGameObject("deepnest_mantis_gate"));
                    }
                    break;
                case SceneNames.Fungus2_25:
                    if (t.GateName.StartsWith("right"))
                    {
                        UObject.Destroy(newScene.FindGameObject("mantis_big_door"));
                    }
                    break;
                case SceneNames.Ruins1_09:
                    if (t.GateName.StartsWith("t"))
                    {
                        UObject.Destroy(newScene.FindGameObject("Battle Gate"));
                        UObject.Destroy(newScene.FindGameObject("Battle Scene"));
                    }
                    break;
                case SceneNames.Waterways_04:
                    if (t.GateName.StartsWith("b"))
                    {
                        foreach (Transform u in newScene.FindGameObject("Quake Floor/Active").transform)
                        {
                            if (u.name.StartsWith("Mask")) u.gameObject.SetActive(false);
                        }
                    }
                    break;
                case SceneNames.White_Palace_03_hub:
                    {
                        UObject.Destroy(newScene.FindGameObject("Progress Gate"));
                        UObject.Destroy(newScene.FindGameObject("Progress Gate (1)"));
                        UObject.Destroy(newScene.FindGameObject("Progress Gate (2)"));
                        UObject.Destroy(newScene.FindGameObject("Progress Gate (3)"));
                    }
                    break;
                case SceneNames.White_Palace_06:
                    UObject.Destroy(newScene.FindGameObject("Path of Pain Blocker"));
                    break;
            }
        }
    }
}
