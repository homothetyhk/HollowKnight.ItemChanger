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
    /// Location which gives an item through Sly Basement dialogue, and triggers a scene change to Dirtmouth.
    /// </summary>
    public class NailmastersGloryLocation : AutoLocation
    {
        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("Sly Basement NPC", "Conversation Control"), EditSlyConvo);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("Sly Basement NPC", "Conversation Control"), EditSlyConvo);
        }

        private void EditSlyConvo(PlayMakerFSM fsm)
        {
            FsmState convo = fsm.GetState("Convo Choice");
            FsmState give = fsm.GetState("Give");
            FsmState end = fsm.GetState("End");

            convo.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)convo.Actions[0]);

            give.Actions = new FsmStateAction[]
            {
                    new AsyncLambda(GiveAll),
            };

            end.AddFirstAction(new ChangeSceneAction("Town", "door_sly"));
        }
    }
}
