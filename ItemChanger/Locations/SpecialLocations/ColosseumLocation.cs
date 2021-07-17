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
using ItemChanger.Internal;

namespace ItemChanger.Locations.SpecialLocations
{
    public class ColosseumLocation : FsmObjectLocation, ILocalHintLocation
    {
        [System.ComponentModel.DefaultValue(true)]
        public bool HintActive { get; set; } = true;
        public bool preOpenTrial = true;

        public override void OnLoad()
        {
            base.OnLoad();
            if (preOpenTrial)
            {
                PlayerData.instance.SetBool(GetOpenBoolName(), true);
            }
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.FsmName)
            {
                case "Battle Control" when fsm.gameObject.name == "Colosseum Manager":
                    {
                        FsmState wave1 = fsm.GetState("Wave 1");
                        wave1.ClearTransitions();
                        wave1.AddTransition("WAVE END", "End");
                    }
                    break;

                case "Geo Pool" when fsm.gameObject.name == "Colosseum Manager":
                    {
                        FsmState openGates = fsm.GetState("Open Gates");
                        openGates.AddFirstAction(new Lambda(SetCompletionBool));
                        FsmState giveShiny = fsm.GetState("Give Shiny?");
                        giveShiny.Actions = new[]
                        {
                            giveShiny.Actions[0], // CROWD IDLE
                            // giveShiny.Actions[1], // bool test on FsmBool Shiny Item
                            new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)giveShiny.Actions[2]),
                            // giveShiny.Actions[3], // find child
                            giveShiny.Actions[4], // activate Shiny Obj
                        };
                        giveShiny.AddTransition("FINISHED", "Geo Given Pause");
                    }
                    break;
            }
        }

        private string GetOpenBoolName()
        {
            switch (sceneName)
            {
                default:
                case SceneNames.Room_Colosseum_Bronze:
                    return nameof(PlayerData.colosseumBronzeOpened);
                case SceneNames.Room_Colosseum_Silver:
                    return nameof(PlayerData.colosseumSilverOpened);
                case SceneNames.Room_Colosseum_Gold:
                    return nameof(PlayerData.colosseumGoldOpened);
            }
        }

        private void SetCompletionBool()
        {
            PlayerData.instance.SetBool(GetCompletionBoolName(), true);
        }

        private string GetCompletionBoolName()
        {
            switch (sceneName)
            {
                default:
                case SceneNames.Room_Colosseum_Bronze:
                    return nameof(PlayerData.colosseumBronzeCompleted);
                case SceneNames.Room_Colosseum_Silver:
                    return nameof(PlayerData.colosseumSilverCompleted);
                case SceneNames.Room_Colosseum_Gold:
                    return nameof(PlayerData.colosseumGoldCompleted);
            }
        }

        private string GetTrialBoardConvo()
        {
            switch (sceneName)
            {
                default:
                case SceneNames.Room_Colosseum_Bronze:
                    return "TRIAL_BOARD_BRONZE";
                case SceneNames.Room_Colosseum_Silver:
                    return "TRIAL_BOARD_SILVER";
                case SceneNames.Room_Colosseum_Gold:
                    return "TRIAL_BOARD_GOLD";
            }
        }

        private string GetTrialBoardHint()
        {
            StringBuilder sb = new StringBuilder();
            switch (sceneName)
            {
                default:
                case SceneNames.Room_Colosseum_Bronze:
                    sb.Append("Trial of the Warrior. ");
                    break;
                case SceneNames.Room_Colosseum_Silver:
                    sb.Append("Trial of the Conqueror. ");
                    break;
                case SceneNames.Room_Colosseum_Gold:
                    sb.Append("Trial of the Fool. ");
                    break;
            }

            sb.AppendLine($"Fight for {Placement.GetUIName(30)} and geo.");
            sb.Append("Place a mark and begin the Trial?");
            return sb.ToString();
        }

        private string GetTrialBoardNullHint()
        {
            StringBuilder sb = new StringBuilder();
            switch (sceneName)
            {
                default:
                case SceneNames.Room_Colosseum_Bronze:
                    sb.Append("Trial of the Warrior. ");
                    break;
                case SceneNames.Room_Colosseum_Silver:
                    sb.Append("Trial of the Conqueror. ");
                    break;
                case SceneNames.Room_Colosseum_Gold:
                    sb.Append("Trial of the Fool. ");
                    break;
            }

            sb.AppendLine($"Fight for geo.");
            sb.Append("Place a mark and begin the Trial?");
            return sb.ToString();
        }

        public override string OnLanguageGet(string convo, string sheet)
        {
            if (convo == GetTrialBoardConvo() && sheet == "Prompts")
            {
                if (HintActive && !Placement.AllObtained())
                {
                    return GetTrialBoardHint();
                }
                else return GetTrialBoardNullHint();
            }

            return base.OnLanguageGet(convo, sheet);
        }

    }
}
