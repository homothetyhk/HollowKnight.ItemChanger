using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which unlocks the Colosseum boards and makes it possible to interact with them regardless of which trials have been completed.
    /// </summary>
    [DefaultModule]
    public class NonlinearColosseums : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Room_Colosseum_01, new("Silver Trial Board", "Conversation Control"), UnlockBoard);
            Events.AddFsmEdit(SceneNames.Room_Colosseum_01, new("Gold Trial Board", "Conversation Control"), UnlockBoard);
            SetColosseumsOpened();
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Room_Colosseum_01, new("Silver Trial Board", "Conversation Control"), UnlockBoard);
            Events.RemoveFsmEdit(SceneNames.Room_Colosseum_01, new("Gold Trial Board", "Conversation Control"), UnlockBoard);
        }

        private void SetColosseumsOpened()
        {
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumBronzeOpened), true);
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumSilverOpened), true);
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumGoldOpened), true);
        }

        private void UnlockBoard(PlayMakerFSM fsm)
        {
            FsmState stateCheck = fsm.GetState("State Check");
            if (stateCheck != null) stateCheck.RemoveTransitionsOn("LOCKED");
        }

    }
}
