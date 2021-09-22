using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    public class DreamNailCutsceneEvent : Module
    {
        /// <summary>
        /// If true, interacting with the plaque no longer warps.
        /// </summary>
        public bool Closed = false;

        /// <summary>
        /// If true, the binding shield is not activated.
        /// </summary>
        public bool Passible = true;

        /// <summary>
        /// If true, the plaque inspection/cutscene is skipped and interacting warps instantly.
        /// </summary>
        public bool Faster = true;

        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.RestingGrounds_04, new("Binding Shield Activate", "FSM"), FixBindingShield);
            Events.AddFsmEdit(SceneNames.RestingGrounds_04, new("PreDreamnail", "FSM"), FixPreDreamnail);
            Events.AddFsmEdit(SceneNames.RestingGrounds_04, new("PostDreamnail", "FSM"), FixPostDreamnail);
            Events.AddFsmEdit(SceneNames.RestingGrounds_04, new("Dreamer Plaque Inspect", "Conversation Control"), FixPlaqueInspect);
            Events.AddFsmEdit(SceneNames.RestingGrounds_04, new("Dreamer Scene 2", "Control"), FixDreamerScene2);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_04, new("Binding Shield Activate", "FSM"), FixBindingShield);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_04, new("PreDreamnail", "FSM"), FixPreDreamnail);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_04, new("PostDreamnail", "FSM"), FixPostDreamnail);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_04, new("Dreamer Plaque Inspect", "Conversation Control"), FixPlaqueInspect);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_04, new("Dreamer Scene 2", "Control"), FixDreamerScene2);
        }


        private void FixBindingShield(PlayMakerFSM fsm)
        {
            if (!Passible)
            {
                FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Check"));
            }
            else
            {
                fsm.GetState("Check").Transitions[0].FsmEvent = FsmEvent.Finished;
            }
        }

        private void FixPreDreamnail(PlayMakerFSM fsm)
        {
            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Check"));
        }

        private void FixPostDreamnail(PlayMakerFSM fsm)
        {
            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Check"));
        }

        private void FixPlaqueInspect(PlayMakerFSM fsm)
        {
            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("End"));
            if (Faster)
            {
                FsmState heroAnim = fsm.GetState("Hero Anim");
                FsmState mapMsg = fsm.GetState("Map Msg?");

                heroAnim.RemoveActionsOfType<ActivateGameObject>();
                heroAnim.Transitions[0].SetToState(mapMsg);
            }
        }

        private void FixDreamerScene2(PlayMakerFSM fsm)
        {
            FixDreamNailCutsceneBoolTest(fsm, fsm.GetState("Init"));
            if (Faster)
            {
                FsmState takeControl = fsm.GetState("Take Control");
                FsmState fadeOut = fsm.GetState("Fade Out");
                FsmState setCompassPoint = fsm.GetState("Set Compass Point");

                takeControl.Transitions[0].SetToState(fadeOut);
                fadeOut.Transitions[0].SetToState(setCompassPoint);
            }
        }

        private void FixDreamNailCutsceneBoolTest(PlayMakerFSM fsm, FsmState state)
        {
            PlayerDataBoolTest pdBoolTest = state.GetActionsOfType<PlayerDataBoolTest>()[0];
            FsmStateAction action = new Lambda(() => fsm.SendEvent(
                Closed ? pdBoolTest.isTrue?.Name : pdBoolTest.isFalse?.Name
                ));
            state.AddFirstAction(action);
            state.RemoveActionsOfType<PlayerDataBoolTest>();
        }
    }
}
