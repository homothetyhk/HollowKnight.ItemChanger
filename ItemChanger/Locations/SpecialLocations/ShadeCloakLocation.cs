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
    public class ShadeCloakLocation : FsmLocation
    {
        public override MessageType MessageType => MessageType.Any;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.FsmName)
            {
                case "Get Shadow Dash" when fsm.gameObject.name == "Dish Plat":
                    {
                        Transform = fsm.transform;

                        FsmState init = fsm.GetState("Init");
                        init.Actions[0] = new BoolTestMod(Placement.AllObtained, init.Actions[0] as PlayerDataBoolTest);

                        FsmState takeControl = fsm.GetState("Take Control");
                        takeControl.RemoveActionsOfType<ActivateGameObject>();

                        FsmState setRespawn = fsm.GetState("Set Respawn");
                        setRespawn.Actions = new FsmStateAction[0];

                        FsmState pd = fsm.GetState("PlayerData");
                        pd.Actions = new FsmStateAction[0];

                        FsmState uiMsg = fsm.GetState("UI Msg");
                        Lambda give = new Lambda(() => Placement.GiveAll(MessageType, () => fsm.Fsm.Event("GET ITEM MSG END")));
                        uiMsg.Actions = new[] { give };

                        FsmState end = fsm.GetState("End");
                        end.RemoveActionsOfType<SendEventToRegister>();
                    }
                    break;
            }
        }


    }
}
