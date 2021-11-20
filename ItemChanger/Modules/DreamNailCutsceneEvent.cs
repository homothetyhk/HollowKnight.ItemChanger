using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which modifies the Dream Nail cutscene to be independent of having the Dream Nail item.
    /// </summary>
    [DefaultModule]
    public class DreamNailCutsceneEvent : Module
    {
        /// <summary>
        /// If evaluates true, interacting with the plaque no longer warps. If null, plaque always warps.
        /// <br/>Default test is true iff "Dream_Nail" placement exists and is cleared or "Dream_Nail" placement does not exist and player has Dream Nail.
        /// </summary>
        public IBool Closed = new PlacementAllObtainedBool(placementName: LocationNames.Dream_Nail, missingPlacementTest: new PDBool(nameof(PlayerData.hasDreamNail)));

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
            PlayerDataBoolTest pdBoolTest = state.GetFirstActionOfType<PlayerDataBoolTest>();
            state.AddFirstAction(new DelegateBoolTest(() => Closed?.Value ?? false, pdBoolTest.isTrue, pdBoolTest.isFalse));
            state.RemoveActionsOfType<PlayerDataBoolTest>();
        }
    }
}
