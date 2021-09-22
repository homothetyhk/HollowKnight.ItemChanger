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

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), EditCrystalShaman);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), EditCrystalShaman);
        }

        private void EditCrystalShaman(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            FsmState get = fsm.GetState("Get PlayerData 2");
            FsmState callUI = fsm.GetState("Call UI Msg 2");

            FsmStateAction check = new BoolTestMod(Placement.AllObtained, "BROKEN", null);
            FsmStateAction give = new AsyncLambda(GiveAllAsync(fsm.transform), "GET ITEM MSG END");

            init.RemoveActionsOfType<IntCompare>();
            init.AddLastAction(check);

            get.Actions = new FsmStateAction[0];
            callUI.Actions = new[] { give };
        }
    }
}
