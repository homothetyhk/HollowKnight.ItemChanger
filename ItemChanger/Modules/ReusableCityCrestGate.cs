using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which allows the City Crest gate to be reused, and prevents its hard save.
    /// </summary>
    [DefaultModule]
    public class ReusableCityCrestGate : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Fungus2_21, new("City Gate Control", "Conversation Control"), RemoveGateInteractActions);
            Events.AddFsmEdit(SceneNames.Fungus2_21, new("Ruins_gate_main", "Open"), RemoveGateSlamActions);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Fungus2_21, new("City Gate Control", "Conversation Control"), RemoveGateInteractActions);
            Events.RemoveFsmEdit(SceneNames.Fungus2_21, new("Ruins_gate_main", "Open"), RemoveGateSlamActions);
        }

        private void RemoveGateInteractActions(PlayMakerFSM fsm)
        {
            fsm.GetState("Activate").RemoveActionsOfType<SetPlayerDataBool>();
        }

        private void RemoveGateSlamActions(PlayMakerFSM fsm)
        {
            FsmState gateSlam = fsm.GetState("Slam");
            gateSlam.RemoveActionsOfType<SetPlayerDataBool>();
            gateSlam.RemoveActionsOfType<CallMethodProper>();
            gateSlam.RemoveActionsOfType<SendMessage>();
        }
    }
}
