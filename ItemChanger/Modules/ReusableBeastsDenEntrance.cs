using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using Modding;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which allows the bench entrance to Beast's Den to be reused, and optionally (disabled by default) prevents its hard save.
    /// </summary>
    [DefaultModule]
    public class ReusableBeastsDenEntrance : Module
    {
        public bool PreventHardSave = false;

        public override void Initialize()
        {
            ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
            Events.AddFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        public override void Unload()
        {
            ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
            Events.RemoveFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        private bool GetPlayerBoolHook(string name, bool orig)
        {
            if (name == nameof(PlayerData.spiderCapture)) return false;
            return orig;
        }

        private void PreventBeastDenCutsceneSave(PlayMakerFSM fsm)
        {
            if (PreventHardSave)
            {
                FsmState denHardSave = fsm.GetState("Land");
                denHardSave.RemoveActionsOfType<CallMethodProper>();
                denHardSave.RemoveActionsOfType<SendMessage>();
                denHardSave.RemoveActionsOfType<SetPlayerDataBool>();
            }
        }
    }
}
