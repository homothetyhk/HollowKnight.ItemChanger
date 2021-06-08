using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;

namespace ItemChanger
{
    public static class WorldEventManager
    {
        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += OnEnable;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= OnEnable;
        }

        private static void OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            switch (self.gameObject.scene.name)
            {
                case SceneNames.RestingGrounds_04:
                    FixDreamNailCutsceneEntry(self);
                    break;
                case SceneNames.Room_Sly_Storeroom:
                    // TODO: implement sly basement sequence
                    break;
                case SceneNames.Room_shop:
                    // TODO: implement unrescued sly
                    break;
            }
        }


        private static void FixDreamNailCutsceneEntry(PlayMakerFSM fsm)
        {
            switch (fsm.gameObject.name)
            {
                default:
                    return;
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
        }

        private static void FixDreamNailCutsceneBoolTest(PlayMakerFSM fsm, FsmState state)
        {
            PlayerDataBoolTest pdBoolTest = state.GetActionsOfType<PlayerDataBoolTest>()[0];
            FsmStateAction action = new RandomizerExecuteLambda(() => fsm.SendEvent(
                Ref.WORLD.dreamNailCutsceneCompleted ? pdBoolTest.isTrue?.Name : pdBoolTest.isFalse?.Name
                ));
            state.AddFirstAction(action);
            state.RemoveActionsOfType<PlayerDataBoolTest>();
        }




    }
}
