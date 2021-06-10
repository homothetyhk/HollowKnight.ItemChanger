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

namespace ItemChanger.Locations
{
    public class DescendingDarkLocation : FsmLocation
    {
        public string objectName;
        public string fsmName;

        public override void OnEnable(PlayMakerFSM fsm, Func<bool> boolTest, Action<Action> giveAction)
        {
            if (fsm.gameObject.name == objectName && fsm.FsmName == fsmName)
            {
                FsmState init = fsm.GetState("Init");
                FsmState get = fsm.GetState("Get PlayerData 2");
                FsmState callUI = fsm.GetState("Call UI Msg 2");

                FsmStateAction check = new Lambda(() => fsm.SendEvent(boolTest() ? "BROKEN" : null));
                Action callback = () => fsm.SendEvent("GET ITEM MSG END");
                FsmStateAction give = new Lambda(() => giveAction.Invoke(callback));

                init.RemoveActionsOfType<IntCompare>();
                init.AddAction(check);

                get.Actions = new FsmStateAction[0];
                callUI.Actions = new[] { give };
            }
        }

        public override Transform FindTransformInScene()
        {
            return ObjectLocation.FindGameObject(objectName)?.transform;
        }

    }
}
