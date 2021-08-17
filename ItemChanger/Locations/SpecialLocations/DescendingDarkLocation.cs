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

namespace ItemChanger.Locations.SpecialLocations
{
    public class DescendingDarkLocation : AutoLocation
    {
        public string objectName;
        public string fsmName;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.gameObject.name == objectName && fsm.FsmName == fsmName)
            {
                Transform = fsm.transform;

                FsmState init = fsm.GetState("Init");
                FsmState get = fsm.GetState("Get PlayerData 2");
                FsmState callUI = fsm.GetState("Call UI Msg 2");

                FsmStateAction check = new BoolTestMod(Placement.AllObtained, "BROKEN", null);
                void Callback()
                {
                    fsm.SendEvent("GET ITEM MSG END");
                }

                FsmStateAction give = new AsyncLambda(GiveAll, "GET ITEM MSG END");

                init.RemoveActionsOfType<IntCompare>();
                init.AddLastAction(check);

                get.Actions = new FsmStateAction[0];
                callUI.Actions = new[] { give };
            }
        }
    }
}
