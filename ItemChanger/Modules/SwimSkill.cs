using GlobalEnums;
using ItemChanger.Extensions;
#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which implements the swim custom skill.
    /// </summary>
    public class SwimSkill : Module
    {
        public bool canSwim { get; set; }

        public override void Initialize()
        {
            Events.AddFsmEdit(new("Surface Water Region"), EditWaterSurface);
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Surface Water Region"), EditWaterSurface);
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
        }

        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(canSwim) => canSwim,
                _ => value,
            };
        }

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(canSwim):
                    canSwim = value;
                    break;
            }
            return value;
        }

        private void EditWaterSurface(PlayMakerFSM fsm)
        {
            if (fsm.gameObject.LocateFSM("Acid Armour Check") != null) return; // acid

            FsmState splash = fsm.GetState("Big Splash?");
            FsmStateAction acidDeath = new FsmStateActions.Lambda(() =>
            {
                if (!canSwim)
                {
                    // this is actually the spike death despite the enum, because the acid death splashes green stuff
                    HeroController.instance.TakeDamage(fsm.gameObject, CollisionSide.other, 1, (int)HazardType.ACID);
                    PlayMakerFSM.BroadcastEvent("SWIM DEATH");
                }
            });

            splash.AddFirstAction(acidDeath);
            splash.AddTransition("SWIM DEATH", "Idle");
        }
    }
}
