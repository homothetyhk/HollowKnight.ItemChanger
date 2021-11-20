using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which allows the bench entrance to Beast's Den to be reused, and prevents its hard save.
    /// </summary>
    [DefaultModule]
    public class ReusableBeastsDenEntrance : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        private void PreventBeastDenCutsceneSave(PlayMakerFSM fsm)
        {
            FsmState denHardSave = fsm.GetState("Land");
            denHardSave.RemoveActionsOfType<CallMethodProper>();
            denHardSave.RemoveActionsOfType<SendMessage>();
            denHardSave.RemoveActionsOfType<SetPlayerDataBool>();
        }

    }
}
