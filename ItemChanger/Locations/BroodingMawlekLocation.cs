using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class BroodingMawlekLocation : ObjectLocation
    {
        public override void PlaceContainer(GameObject obj, Container containerType)
        {
            base.PlaceContainer(obj, containerType);

            GameObject battleScene = GameObject.Find("Battle Scene");
            PlayMakerFSM control = battleScene.LocateFSM("Battle Control");
            control.FsmVariables.FindFsmGameObject("Heart Piece").Value = obj;
        }

        public override void OnEnable(PlayMakerFSM fsm)
        {
            switch (fsm.FsmName)
            {
                case "Battle Control" when fsm.gameObject.name == "Battle Scene":
                    {
                        FsmState prepause = fsm.GetState("PrePause");
                        FsmState endwait = fsm.GetState("End Wait");
                        FsmState activate = fsm.GetState("Activate");

                        prepause.RemoveActionsOfType<FindGameObject>();

                        endwait.RemoveActionsOfType<SetFsmGameObject>();
                        endwait.RemoveActionsOfType<SetFsmBool>();

                        activate.RemoveActionsOfType<SetFsmBool>();
                    }
                    break;
            }
        }
    }
}
