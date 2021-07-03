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
using SereCore;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class VoidHeartLocation : FsmLocation
    {
        public override MessageType MessageType => MessageType.Any;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.gameObject.name)
            {
                case "End Cutscene" when fsm.FsmName == "Control":
                    {
                        FsmState charmPause = fsm.GetState("Charm Pause");
                        FsmState charmGet = fsm.GetState("Charm Get");
                        FsmState removeOvercharm = fsm.GetState("Remove Overcharm");
                        FsmState getMsg = fsm.GetState("Get Msg");

                        FsmStateAction give = new Lambda(() => Placement.GiveAll(MessageType, () => fsm.Fsm.Event("GET ITEM MSG END")));

                        charmPause.Actions = new FsmStateAction[0];
                        charmGet.Actions = new FsmStateAction[0];
                        removeOvercharm.Actions = new FsmStateAction[0];
                        getMsg.Actions = new[] { give };
                    }
                    break;
            }
        }

        public override void OnEnableGlobal(PlayMakerFSM fsm)
        {
            base.OnEnableGlobal(fsm);
            if (fsm.gameObject.scene.name == "Abyss_15")
            {
                switch (fsm.gameObject.name)
                {
                    case "Dream Enter Abyss" when fsm.FsmName == "Control":
                        {
                            FsmState init = fsm.GetState("Init");
                            init.Actions = new[] { init.Actions[0], new BoolTestMod(Placement.AllObtained, "INACTIVE", null) };
                        }
                        break;
                    case "Mirror" when fsm.FsmName == "FSM":
                        {
                            FsmState check = fsm.GetState("Check");
                            check.Actions[0] = new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
                        }
                        break;
                }
            }
        }
    }
}
