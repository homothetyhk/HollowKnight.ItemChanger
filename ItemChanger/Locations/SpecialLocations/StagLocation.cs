using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    public class StagLocation : CoordinateLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            if (fsm.FsmName == "Stag Bell")
            {
                switch (fsm.gameObject.name)
                {
                    case "Station Bell":
                    case "Station Bell Lever": // RG
                        FsmState init = fsm.GetState("Init");
                        init.RemoveActionsOfType<PlayerDataBoolTest>();
                        init.AddTransition("FINISHED", "Opened");
                        break;
                }
            }
            else if (fsm.FsmName == "Stag Control" && fsm.gameObject.name == "Stag")
            {
                FsmState hsprompt = fsm.GetState("Hidden Station?");
                FsmState openGrate = fsm.GetState("Open Grate");
                FsmState currentLocationCheck = fsm.GetState("Current Location Check");
                FsmState checkResult = fsm.GetState("Check Result");
                FsmState hudreturn = fsm.GetState("HUD Return");

                if (sceneName == "Abyss_22") hsprompt.RemoveActionsOfType<IntCompare>();
                
                openGrate.RemoveActionsOfType<SetPlayerDataBool>();
                openGrate.RemoveActionsOfType<SetBoolValue>();

                FsmBool cancelTravel = fsm.AddFsmBool("Cancel Travel", false);

                if (!PlayerData.instance.GetBool(fsm.FsmVariables.StringVariables.First(v => v.Name == "Station Opened Bool").Value))
                {
                    fsm.FsmVariables.IntVariables.First(v => v.Name == "Station Position Number").Value = 0;
                    currentLocationCheck.RemoveActionsOfType<IntCompare>();
                    
                    checkResult.AddFirstAction(new Lambda(() =>
                    {
                        if (cancelTravel.Value)
                        {
                            fsm.SendEvent("CANCEL");
                        }
                    }));
                    checkResult.AddTransition("CANCEL", "HUD Return");
                }

                fsm.GetState("HUD Return").AddFirstAction(new SetBoolValue
                {
                    boolVariable = cancelTravel,
                    boolValue = false
                });
            }
            else if (fsm.FsmName == "ui_list" && fsm.gameObject.name == "UI List Stag")
            {
                fsm.GetState("Selection Made Cancel").AddFirstAction(new Lambda(() =>
                {
                    GameObject.Find("Stag").LocateMyFSM("Stag Control").FsmVariables
                        .BoolVariables.First(v => v.Name == "Cancel Travel").Value = true;
                }));
            }
            else if (fsm.FsmName == "Switch Control" && fsm.gameObject.name == "Ruins Lever" && sceneName == "RestingGrounds_09")
            {
                UnityEngine.Object.Destroy(fsm.gameObject);
            }
        }


    }
}
