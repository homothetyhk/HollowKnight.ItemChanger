using System.Text;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// FsmObjectLocation with various changes to support items at the end of a Colosseum trial.
    /// </summary>
    public class ColosseumLocation : FsmObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Colosseum Manager", "Geo Pool"), ChangeColoEnd);
            Events.AddLanguageEdit(new("Prompts", GetTrialBoardConvo()), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Colosseum Manager", "Geo Pool"), ChangeColoEnd);
            Events.RemoveLanguageEdit(new("Prompts", GetTrialBoardConvo()), OnLanguageGet);
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
                new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)giveShiny.Actions[2]),
                // giveShiny.Actions[3], // find child
                giveShiny.Actions[4], // activate Shiny Obj
            };
            giveShiny.AddTransition("FINISHED", "Geo Given Pause");
        }

        private void SetCompletionBool()
        {
            PlayerData.instance.SetBool(GetCompletionBoolName(), true);
        }

        private string GetCompletionBoolName() => sceneName switch
        {
            SceneNames.Room_Colosseum_Bronze => nameof(PlayerData.colosseumBronzeCompleted),
            SceneNames.Room_Colosseum_Silver => nameof(PlayerData.colosseumSilverCompleted),
            _ => nameof(PlayerData.colosseumGoldCompleted),
        };

        private string GetTrialBoardConvo() => sceneName switch
        {
            SceneNames.Room_Colosseum_Bronze => "TRIAL_BOARD_BRONZE",
            SceneNames.Room_Colosseum_Silver => "TRIAL_BOARD_SILVER",
            _ => "TRIAL_BOARD_GOLD",
        };

        private string GetTrialHintConvo() => sceneName switch
        {
            SceneNames.Room_Colosseum_Bronze => "TRIAL_HINT_BRONZE",
            SceneNames.Room_Colosseum_Silver => "TRIAL_HINT_SILVER",
            _ => "TRIAL_HINT_GOLD",
        };

        private string GetTrialNullHintConvo() => sceneName switch
        {
            SceneNames.Room_Colosseum_Bronze => "TRIAL_NULLHINT_BRONZE",
            SceneNames.Room_Colosseum_Silver => "TRIAL_NULLHINT_SILVER",
            _ => "TRIAL_NULLHINT_GOLD",
        };

        private void OnLanguageGet(ref string value)
        {
            if (this.GetItemHintActive() && !Placement.AllObtained())
            {
                string text = Placement.GetUIName(75);
                value = string.Format(Language.Language.Get(GetTrialHintConvo(), "Fmt"), text);
                Placement.OnPreview(text);
            }
            else value = Language.Language.Get(GetTrialNullHintConvo(), "Prompts");
        }
    }
}
