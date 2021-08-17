using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;
using UnityEngine;
using Modding;

namespace ItemChanger.Internal
{
    public static class WorldEventManager
    {
        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += OnEnable;
            ModHooks.GetPlayerBoolHook += OverrideGetBool;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= OnEnable;
        }

        private static bool OverrideGetBool(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(PlayerData.gotSlyCharm):
                    return Ref.WORLD.slyBasementCompleted;
                case nameof(PlayerData.hasAllNailArts):
                    return PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                        && PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                        && PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash));
                case nameof(PlayerData.hasNailArt):
                    return PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                        || PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                        || PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash));
                default:
                    return value;
            }
        }

        private static void OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);
            switch (fsm.gameObject.scene.name)
            {
                case SceneNames.RestingGrounds_04:
                    switch (fsm.gameObject.name)
                    {
                        default:
                            break;
                        case "Binding Shield Activate" when fsm.FsmName == "FSM":
                        case "PreDreamnail" when fsm.FsmName == "FSM":
                        case "PostDreamnail" when fsm.FsmName == "FSM":
                            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Check"));
                            break;
                        case "Dreamer Plaque Inspect" when fsm.FsmName == "Conversation Control":
                            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("End"));
                            break;
                        case "Dreamer Scene 2" when fsm.FsmName == "Control":
                            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Init"));
                            break;
                    }
                    break;
                case SceneNames.Room_ruinhouse:
                    switch (fsm.gameObject.name)
                    {
                        default:
                            break;
                        case "Sly Dazed" when fsm.FsmName == "Conversation Control":
                            FixDazedSlyBool(fsm);
                            break;
                    }
                    break;
                case SceneNames.Room_shop when !Ref.WORLD.slyRescued:
                    switch (fsm.gameObject.name)
                    {
                        case "Sly Shop":
                        case "Shop Region":
                        case "Basement Open":
                        case "door1":
                            UnityEngine.Object.Destroy(fsm.gameObject);
                            break;
                    }
                    break;
                case SceneNames.Room_Colosseum_01 when Ref.WORLD.nonlinearColosseums:
                    if (fsm.FsmName == "Conversation Control")
                    {
                        switch (fsm.gameObject.name)
                        {
                            case "Silver Trial Board":
                            case "Gold Trial Board":
                                {
                                    FsmState stateCheck = fsm.GetState("State Check");
                                    if (stateCheck != null) stateCheck.RemoveTransitionsOn("LOCKED");
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private static void FixDazedSlyBool(PlayMakerFSM dazedSly)
        {
            FsmState active = dazedSly.GetState("Active?");
            FsmState convo = dazedSly.GetState("Convo Choice");
            FsmState meet = dazedSly.GetState("Meet");

            if (active.GetFirstActionOfType<BoolTest>() is BoolTest test1)
            {
                active.AddLastAction(new Lambda(() => dazedSly.SendEvent(Ref.WORLD.slyRescued ? test1.isTrue?.Name : test1.isFalse?.Name)));
                active.RemoveActionsOfType<BoolTest>();
            }

            if (convo.GetFirstActionOfType<BoolTest>() is BoolTest test2)
            {
                active.AddLastAction(new Lambda(() => dazedSly.SendEvent(Ref.WORLD.slyRescued ? test2.isTrue?.Name : test2.isFalse?.Name)));
                active.RemoveActionsOfType<BoolTest>();
            }

            meet.AddLastAction(new Lambda(() => Ref.WORLD.slyRescued = true));
        }


        private static void FixDreamNailCutsceneBoolTest(PlayMakerFSM fsm, FsmState state)
        {
            PlayerDataBoolTest pdBoolTest = state.GetActionsOfType<PlayerDataBoolTest>()[0];
            FsmStateAction action = new Lambda(() => fsm.SendEvent(
                Ref.WORLD.dreamNailCutsceneCompleted ? pdBoolTest.isTrue?.Name : pdBoolTest.isFalse?.Name
                ));
            state.AddFirstAction(action);
            state.RemoveActionsOfType<PlayerDataBoolTest>();
        }




    }
}
