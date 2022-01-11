using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which prevents Leg Eater from leaving when Divine's questline is finished.
    /// </summary>
    [DefaultModule]
    public class PreventLegEaterDeath : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Fungus2_26, new("Leg Eater", "Conversation Control"), RemoveConversationOptions);
            Modding.ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Fungus2_26, new("Leg Eater", "Conversation Control"), RemoveConversationOptions);
            Modding.ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
        }

        private bool GetPlayerBoolHook(string name, bool orig)
        {
            if (name == nameof(PlayerData.legEaterLeft)) return false; // this shouldn't actually be necessary, but better safe than sorry
            else return orig;
        }

        private void RemoveConversationOptions(PlayMakerFSM fsm)
        {
            FsmState legEaterChoice = fsm.GetState("Convo Choice");
            legEaterChoice.RemoveAction(2); // remove the check for the all unbreakables convo

            FsmState allGold = fsm.GetState("All Gold?");
            allGold.RemoveTransitionsTo("No Shop"); // remove the check which deactivates the shop
        }
    }
}
