using ItemChanger.Extensions;
using Modding;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which prevents the Grimm and Divine tents from leaving Dirtmouth.
    /// </summary>
    [DefaultModule]
    public class PreventGrimmTroupeLeave : Module
    {
        public override void Initialize()
        {
            ModHooks.SetPlayerBoolHook += OverrideBoolSet;
            Events.AddFsmEdit(SceneNames.Town, new("main_tent", "FSM"), PreventGrimmTentLeave);
        }

        public override void Unload()
        {
            ModHooks.SetPlayerBoolHook -= OverrideBoolSet;
            Events.RemoveFsmEdit(SceneNames.Town, new("main_tent", "FSM"), PreventGrimmTentLeave);
        }

        private void PreventGrimmTentLeave(PlayMakerFSM fsm)
        {
            fsm.GetState("Check").ClearTransitions();
        }

        private bool OverrideBoolSet(string name, bool orig)
        {
            return name switch
            {
                nameof(PlayerData.troupeInTown) => PlayerData.instance.GetBool(name) || orig,
                nameof(PlayerData.divineInTown) => PlayerData.instance.GetBool(name) || orig,
                _ => orig
            };
        }
    }
}
