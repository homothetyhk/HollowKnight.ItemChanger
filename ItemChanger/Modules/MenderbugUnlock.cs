using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which unlocks the door to Menderbug's house by default.
    /// </summary>
    [DefaultModule]
    public class MenderbugUnlock : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Crossroads_04, new("Mender Door", "Check State"), OnEnable);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Crossroads_04, new("Mender Door", "Check State"), OnEnable);
        }

        private void OnEnable(PlayMakerFSM fsm)
        {
            FsmState pause = fsm.GetState("Pause");
            FsmState opened = fsm.GetState("Opened");
            pause.Transitions[0].SetToState(opened);
        }
    }
}
