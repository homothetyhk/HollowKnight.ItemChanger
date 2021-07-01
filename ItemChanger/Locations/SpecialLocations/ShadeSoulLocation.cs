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

namespace ItemChanger.Locations.SpecialLocations
{
    public class ShadeSoulLocation : FsmLocation
    {
        public override MessageType MessageType => MessageType.Any;

        public override void OnEnable(PlayMakerFSM fsm)
        {
            switch (fsm.gameObject.name)
            {
                case "Ruins Shaman" when fsm.FsmName == "Ruins Shaman":
                    {
                        FsmState gotSpell = fsm.GetState("Got Spell?");
                        gotSpell.RemoveActionsOfType<IntCompare>();
                        gotSpell.AddAction(new BoolTestMod(Placement.AllObtained, "ACTIVATED", null));
                    }
                    break;

                case "Knight Get Fireball Lv2" when fsm.FsmName == "Get Fireball":
                    {
                        FsmState getPD = fsm.GetState("Get PlayerData");
                        FsmState UIMsg = fsm.GetState("Call UI Msg");

                        Action callback = () => fsm.Fsm.Event("GET ITEM MSG END");
                        FsmStateAction give = new Lambda(() => Placement.GiveAll(MessageType, callback));

                        getPD.RemoveActionsOfType<SetPlayerDataInt>();
                        UIMsg.Actions = new FsmStateAction[]
                        {
                            give
                        };
                    }
                    break;
            }
        }
    }
}
