using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which gives items when Brumm's Grimmkin flame would be received.
    /// </summary>
    public class BrummFlameLocation : AutoLocation
    {
        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("Brumm Torch NPC", "Conversation Control"), EditBrummConvo);
            Events.AddLanguageEdit(new("CP2", "BRUMM_DEEPNEST_3"), EditBrummText);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("Brumm Torch NPC", "Conversation Control"), EditBrummConvo);
            Events.RemoveLanguageEdit(new("CP2", "BRUMM_DEEPNEST_3"), EditBrummText);
        }

        private void EditBrummConvo(PlayMakerFSM fsm)
        {
            FsmState checkActive = fsm.GetState("Check Active");
            FsmState convo1 = fsm.GetState("Convo 1");
            FsmState get = fsm.GetState("Get");

            checkActive.SetActions(
                new DelegateBoolTest(() => IsBrummActive() && !Placement.AllObtained(), (PlayerDataBoolTest)checkActive.Actions[0])
            );

            convo1.RemoveActionsOfType<IntCompare>();

            get.SetActions(
                get.Actions[6], // set Activated--not used by IC, but preserves grimmkin status if IC is disabled
                get.Actions[14], // set gotBrummsFlame
                new AsyncLambda(GiveAllAsync(fsm.transform))
            );
        }

        private void EditBrummText(ref string value)
        {
            string text = Placement.GetUIName(40);
            value = string.Format(Language.Language.Get("BRUMM_DEEPNEST_3_HINT", "Fmt"), text);
            Placement.OnPreview(text);
        }

        private static bool IsBrummActive()
        {
            int grimmchildLevel = PlayerData.instance.GetInt("grimmChildLevel");
            return PlayerData.instance.GetBool("equippedCharm_40") && grimmchildLevel >= 3; // && !Ref.PD.GetBool("gotBrummsFlame") && grimmchildLevel < 5;
        }
    }
}
