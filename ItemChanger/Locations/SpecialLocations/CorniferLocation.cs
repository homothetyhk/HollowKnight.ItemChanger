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
    /// Location which gives items through the interface to purchase a map from Cornifer.
    /// </summary>
    public class CorniferLocation : AutoLocation
    {
        public string objectName;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, "Conversation Control"), HandleCorniferConvo);
            Events.AddFsmEdit(sceneName, new("Cornifer Card", "FSM"), HandleCorniferCard);
            Events.AddLanguageEdit(new("Cornifer", "CORNIFER_PROMPT"), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, "Conversation Control"), HandleCorniferConvo);
            Events.RemoveFsmEdit(sceneName, new("Cornifer Card", "FSM"), HandleCorniferCard);
            Events.RemoveLanguageEdit(new("Cornifer", "CORNIFER_PROMPT"), OnLanguageGet);
        }

        private void HandleCorniferConvo(PlayMakerFSM fsm)
        {
            FsmState checkActive = fsm.GetState("Check Active");
            FsmState convoChoice = fsm.GetState("Convo Choice");
            FsmState get = fsm.GetState("Geo Pause and GetMap");
            FsmState sendText = fsm.GetState("Send Text");

            checkActive.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)checkActive.Actions[0]);
            convoChoice.Actions[1] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)convoChoice.Actions[1]);
            sendText.AddFirstAction(new Lambda(() => Placement.AddVisitFlag(VisitState.Previewed)));

            get.Actions = new FsmStateAction[]
            {
                get.Actions[0], // Wait
                get.Actions[1], // Box Down
                get.Actions[2], // Npc title down
                // get.Actions[3] // SetPlayerDataBool
                // get.Actions[4-7] // nonDeepnest only, map achievement/messages
                new AsyncLambda(GiveAllAsync(fsm.transform), "TALK FINISH")
            };
            get.ClearTransitions();

            if (fsm.GetState("Deepnest Check") is FsmState deepnestCheck)
            {
                deepnestCheck.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)deepnestCheck.Actions[0]);
            }
        }

        private void HandleCorniferCard(PlayMakerFSM fsm)
        {
            FsmState check = fsm.GetState("Check");
            check.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
        }

        private void OnLanguageGet(ref string value)
        {
            if (GameManager.instance.sceneName == sceneName)
            {
                value = Placement.GetUIName();
            }
        }
    }
}
