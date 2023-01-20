using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which gives items when watching the cutscene at the end of Path of Pain.
    /// </summary>
    public class SealOfBindingLocation : AutoLocation
    {
        public string? objectName;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, "Conversation Control"), EditConvo);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, "Conversation Control"), EditConvo);
        }

        private void EditConvo(PlayMakerFSM fsm)
        {
            Util.TabletUtility.AddItemParticles(fsm.gameObject, Placement, Placement.Items);

            FsmState disappearScne = fsm.GetState("Disappear Scne");
            FsmState fadeOut = fsm.GetState("Fade Out");
            FsmState journal = fsm.GetState("Journal");
            journal.ClearActions();

            FsmState give = new FsmState(journal);
            give.ClearTransitions();
            fsm.AddState(give);
            disappearScne.Transitions[0].SetToState(give);
            give.AddTransition(FsmEvent.Finished, fadeOut);
            give.SetActions(
                new DelegateBoolTest(Placement.AllObtained, FsmEvent.Finished, null),
                new AsyncLambda(GiveAllAsync(fsm.transform))
            );
        }
    }
}
