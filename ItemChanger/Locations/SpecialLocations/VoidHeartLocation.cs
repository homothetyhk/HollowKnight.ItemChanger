using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
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
            FsmState charmPause = fsm.GetState("Charm Pause");
            FsmState charmGet = fsm.GetState("Charm Get");
            FsmState removeOvercharm = fsm.GetState("Remove Overcharm");
            FsmState getMsg = fsm.GetState("Get Msg");

            FsmStateAction give = new AsyncLambda(GiveAll, "GET ITEM MSG END");

            charmPause.Actions = new FsmStateAction[0];
            charmGet.Actions = new FsmStateAction[0];
            removeOvercharm.Actions = new FsmStateAction[0];
            getMsg.Actions = new[] { give };
        }

        private void EditDreamEnter(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.Actions = new[] { init.Actions[0], new BoolTestMod(Placement.AllObtained, "INACTIVE", null) };
        }

        private void EditMirror(PlayMakerFSM fsm)
        {
            if (HintActive) HintBox.Create(fsm.transform, Placement); // TODO: test ingame to see if this extends far enough

            FsmState check = fsm.GetState("Check");
            check.Actions[0] = new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
        }
    }
}
