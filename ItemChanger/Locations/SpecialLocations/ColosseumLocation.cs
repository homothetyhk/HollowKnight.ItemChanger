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
using ItemChanger.Internal;

namespace ItemChanger.Locations.SpecialLocations
{
    public class ColosseumLocation : FsmObjectLocation, ILocalHintLocation
    {
        [System.ComponentModel.DefaultValue(true)]
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Colosseum Manager", "Geo Pool"), ChangeColoEnd);
            Events.AddFsmEdit(sceneName, new("Colosseum Manager", "Battle Control"), SkipColoForTesting);
            Events.AddLanguageEdit(new("Prompts", GetTrialBoardConvo()), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Colosseum Manager", "Geo Pool"), ChangeColoEnd);
            Events.RemoveFsmEdit(sceneName, new("Colosseum Manager", "Battle Control"), SkipColoForTesting);
            Events.RemoveLanguageEdit(new("Prompts", GetTrialBoardConvo()), OnLanguageGet);
        }

        private void SkipColoForTesting(PlayMakerFSM fsm)
        {
            // For testing only! Skip to end after first wave.
            FsmState wave1 = fsm.GetState("Wave 1");
            wave1.ClearTransitions();
            wave1.AddTransition("WAVE END", "End");
        }

        private void ChangeColoEnd(PlayMakerFSM fsm)
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

            sb.AppendLine($"Fight for {Placement.GetUIName(75)} and geo.");
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

        private void OnLanguageGet(ref string value)
        {
            if (HintActive && !Placement.AllObtained())
            {
                value = GetTrialBoardHint();
            }
            else value = GetTrialBoardNullHint();
        }
    }
}
