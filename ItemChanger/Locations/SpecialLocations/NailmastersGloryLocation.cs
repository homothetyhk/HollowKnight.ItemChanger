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
    /// <summary>
    /// Gives item through Sly Basement dialogue, then returns to Dirtmouth.
    /// </summary>
    public class NailmastersGloryLocation : FsmLocation
    {
        public override MessageType MessageType => MessageType.Any;
        public override void OnEnable(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Conversation Control" && fsm.gameObject.name == "Sly Basement NPC")
            {
                FsmState convo = fsm.GetState("Convo Choice");
                FsmState give = fsm.GetState("Give");
                FsmState end = fsm.GetState("End");

                convo.Actions[0] = new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)convo.Actions[0]);

                give.Actions = new FsmStateAction[]
                {
                    new AsyncLambda(callback => Placement.GiveAll(MessageType, callback)),
                };

                end.AddFirstAction(new RandomizerChangeScene("Town", "door_sly"));
            }
        }

    }
}
