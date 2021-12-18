using HutongGames.PlayMaker.Actions;
using Modding;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which prevents the Grimm and Divine tents from leaving Dirtmouth.
    /// </summary>
    public class GrimmTroupeTentUnlock : Module
    {
        public override void Initialize()
        {
            ModHooks.SetPlayerBoolHook += OverrideBoolSet;
        }

        public override void Unload()
        {
            ModHooks.SetPlayerBoolHook -= OverrideBoolSet;
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
