using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Default module which removes the failsafe that gives Dream Gate automatically in saves where the Seer has ascended.
    /// </summary>
    [DefaultModule]
    public class PreventSeerLeaveDreamgate : Module
    {
        public override void Initialize()
        {
            IL.GameManager.CheckAllAchievements += DoNotGiveDreamgate;
        }
        public override void Unload()
        {
            IL.GameManager.CheckAllAchievements -= DoNotGiveDreamgate;
        }

        private void DoNotGiveDreamgate(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(
                i => i.Match(OpCodes.Ldarg_0),
                i => i.MatchLdfld<GameManager>(nameof(GameManager.playerData)),
                i => i.Match(OpCodes.Ldc_I4_1),
                i => i.MatchLdstr(nameof(PlayerData.hasDreamGate)),
                i => i.MatchCallvirt<PlayerData>("SetBoolSwappedArgs")
              ))
            {
                cursor.RemoveRange(5);
            }
        }
    }
}
