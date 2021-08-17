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
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Gives item through Sly Basement dialogue, then returns to Dirtmouth.
    /// </summary>
    public class NailmastersGloryLocation : AutoLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Conversation Control" && fsm.gameObject.name == "Sly Basement NPC")
            {
                FsmState convo = fsm.GetState("Convo Choice");
                FsmState give = fsm.GetState("Give");
                FsmState end = fsm.GetState("End");

                convo.Actions[0] = new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)convo.Actions[0]);

                give.Actions = new FsmStateAction[]
                {
                    new AsyncLambda(GiveAll),
                };

                end.AddFirstAction(new RandomizerChangeScene("Town", "door_sly"));
            }
        }

    }
}
