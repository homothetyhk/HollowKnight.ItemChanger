using ItemChanger.Extensions;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which clears actions and transitions from a state when the attached fsm activates.
    /// <br/>If a state is not specified, disables the Pause, Init, and Idle states if they exist.
    /// </summary>
    public class DisableFsmTag : Tag
    {
        public FsmID id;
        public string? sceneName;
        public string? disableState;

        public override void Load(object parent)
        {
            base.Load(parent);
            if (sceneName == null) Events.AddFsmEdit(id, Disable);
            else Events.AddFsmEdit(sceneName, id, Disable);
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);
            if (sceneName == null) Events.RemoveFsmEdit(id, Disable);
            else Events.RemoveFsmEdit(sceneName, id, Disable);
        }

        private void Disable(PlayMakerFSM fsm)
        {
            if (disableState != null)
            {
                Disable(fsm, disableState);
            }
            else
            {
                Disable(fsm, "Pause");
                Disable(fsm, "Init");
                Disable(fsm, "Idle");
            }
        }

        private void Disable(PlayMakerFSM fsm, string stateName)
        {
            if (fsm.GetState(stateName) is FsmState disable)
            {
                disable.ClearActions();
                disable.ClearTransitions();
            }
        }
    }
}