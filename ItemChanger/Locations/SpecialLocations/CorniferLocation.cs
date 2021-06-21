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
    public class CorniferLocation : FsmLocation
    {
        public string objectName;

        public override void OnEnable(PlayMakerFSM fsm, IFsmLocationActions actions)
        {
            switch (fsm.FsmName)
            {
                case "Conversation Control" when fsm.gameObject.name == objectName:
                    {
                        FsmState checkActive = fsm.GetState("Check Active");
                        FsmState convoChoice = fsm.GetState("Convo Choice");
                        FsmState get = fsm.GetState("Geo Pause and GetMap");

                        checkActive.Actions[0] = new BoolTestMod(actions.AllObtained, (PlayerDataBoolTest)checkActive.Actions[0]);
                        convoChoice.Actions[1] = new BoolTestMod(actions.AllObtained, (PlayerDataBoolTest)convoChoice.Actions[1]);

                        void Callback()
                        {
                            fsm.Fsm.Event("TALK FINISH");
                        }

                        get.Actions = new FsmStateAction[]
                        {
                            get.Actions[0], // Wait
                            get.Actions[1], // Box Down
                            get.Actions[2], // Npc title down
                            // get.Actions[3] // SetPlayerDataBool
                            // get.Actions[4-7] // nonDeepnest only, map achievement/messages
                            new Lambda(() => actions.Give(Callback))
                        };
                        get.ClearTransitions();

                        if (fsm.GetState("Deepnest Check") is FsmState deepnestCheck)
                        {
                            deepnestCheck.Actions[0] = new BoolTestMod(actions.AllObtained, (PlayerDataBoolTest)deepnestCheck.Actions[0]);
                        }
                    }
                    break;
                case "FSM" when fsm.gameObject.name == "Cornifer Card":
                    {
                        FsmState check = fsm.GetState("Check");
                        check.Actions[0] = new BoolTestMod(actions.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
                    }
                    break;
            }
        }

        public override string OnLanguageGet(string convo, string sheet, Func<string> getItemName)
        {
            if (sheet == "Cornifer" && convo == "CORNIFER_PROMPT" && GameManager.instance.sceneName == sceneName)
            {
                return getItemName();
            }

            return base.OnLanguageGet(convo, sheet, getItemName);
        }
    }
}
