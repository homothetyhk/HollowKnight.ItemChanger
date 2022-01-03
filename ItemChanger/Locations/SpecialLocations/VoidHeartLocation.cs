using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which directly gives items in place of Void Heart following the Dream_Abyss sequence.
    /// </summary>
    public class VoidHeartLocation : AutoLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("End Cutscene", "Control"), EditEndCutscene);
            Events.AddFsmEdit(SceneNames.Abyss_15, new("Mirror", "FSM"), EditMirror);
            Events.AddFsmEdit(SceneNames.Abyss_15, new("Dream Enter Abyss", "Control"), EditDreamEnter);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("End Cutscene", "Control"), EditEndCutscene);
            Events.RemoveFsmEdit(SceneNames.Abyss_15, new("Mirror", "FSM"), EditMirror);
            Events.RemoveFsmEdit(SceneNames.Abyss_15, new("Dream Enter Abyss", "Control"), EditDreamEnter);
        }

        private void EditEndCutscene(PlayMakerFSM fsm)
        {
            FsmState charmGet = fsm.GetState("Charm Get");
            FsmState removeOvercharm = fsm.GetState("Remove Overcharm");
            FsmState getMsg = fsm.GetState("Get Msg");

            FsmStateAction give = new AsyncLambda(GiveAll, "GET ITEM MSG END");

            charmGet.Actions = new FsmStateAction[0];
            removeOvercharm.Actions = new FsmStateAction[0];
            getMsg.Actions = new[] { getMsg.Actions[0], give };
        }

        private void EditDreamEnter(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.Actions = new[] { init.Actions[0], new DelegateBoolTest(Placement.AllObtained, "INACTIVE", null) };
        }

        private void EditMirror(PlayMakerFSM fsm)
        {
            if (this.GetItemHintActive()) HintBox.Create(fsm.transform, Placement);

            FsmState check = fsm.GetState("Check");
            check.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
        }
    }
}
