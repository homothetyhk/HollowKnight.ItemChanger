using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class BroodingMawlekLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Battle Scene", "Battle Control"), RemoveHeartPieceActions);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Battle Scene", "Battle Control"), RemoveHeartPieceActions);
        }

        private void RemoveHeartPieceActions(PlayMakerFSM fsm)
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
