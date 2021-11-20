using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// FsmObjectLocation with fsm edits for spawning an item from the Brooding Mawlek fight.
    /// </summary>
    public class BroodingMawlekLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Battle Scene", "Battle Control"), RemoveHeartPieceActions);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Battle Scene", "Battle Control"), RemoveHeartPieceActions);
        }

        private void RemoveHeartPieceActions(PlayMakerFSM fsm)
        {
            switch (fsm.FsmName)
            {
                case "Battle Control" when fsm.gameObject.name == "Battle Scene":
                    {
                        FsmState prepause = fsm.GetState("PrePause");
                        FsmState endwait = fsm.GetState("End Wait");
                        FsmState activate = fsm.GetState("Activate");

                        prepause.RemoveActionsOfType<FindGameObject>();

                        endwait.RemoveActionsOfType<SetFsmGameObject>();
                        endwait.RemoveActionsOfType<SetFsmBool>();

                        activate.RemoveActionsOfType<SetFsmBool>();
                    }
                    break;
            }
        }
    }
}
