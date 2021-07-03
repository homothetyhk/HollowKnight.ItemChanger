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
    public class AbyssShriekLocation : FsmLocation
    {
        public override MessageType MessageType => MessageType.Any;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.gameObject.name)
            {
                case "Scream 2 Get" when fsm.FsmName == "Scream Get":
                    {
                        Transform = fsm.transform;

                        FsmState init = fsm.GetState("Init");
                        init.RemoveActionsOfType<IntCompare>();
                        init.AddFirstAction(new BoolTestMod(Placement.AllObtained, "INERT", null));

                        FsmState uiMsg = fsm.GetState("Ui Msg");
                        Lambda give = new Lambda(() => Placement.GiveAll(MessageType, () => fsm.Fsm.Event("GET ITEM MSG END")));
                        uiMsg.Actions = new[] { give };
                    }
                    break;
            }
        }
    }
}
