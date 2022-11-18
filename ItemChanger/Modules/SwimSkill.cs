using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
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
                    PlayMakerFSM.BroadcastEvent("SWIM GET");
                    break;
            }
            return value;
        }

        private void EditWaterSurface(PlayMakerFSM fsm)
        {
            if (fsm.gameObject.LocateMyFSM("Acid Armour Check") != null) return; // acid

            GameObject splashSurface = fsm.transform.Find("Splash Surface").gameObject;
            splashSurface.layer = 17; // orig is 8, which can enable seam jumping when it intersects with other terrain colliders
            splashSurface.AddComponent<NonBouncer>();

            FsmState idle = fsm.GetState("Idle");
            FsmState checkSwim = fsm.AddState("Check Swim");
            FsmState damageHero = fsm.AddState("Damage Hero");
            FsmState bigSplash = fsm.GetState("Big Splash?");

            idle.Transitions[0].SetToState(checkSwim);
            checkSwim.AddFirstAction(new DelegateBoolTest(() => canSwim, "SWIM", "DAMAGE"));
            checkSwim.AddTransition("SWIM", bigSplash);
            checkSwim.AddTransition("DAMAGE", damageHero);

            damageHero.SetActions(fsm.GetState("Splash In Norm").Actions.Where(a => a is not SetPosition).ToArray()); // play splash audio and fling splash particles
            damageHero.AddLastAction(new Lambda(() => HeroController.instance.TakeDamage(fsm.gameObject, CollisionSide.bottom, 1, 2)));
            damageHero.AddTransition(FsmEvent.Finished, idle);
        }
    }
}
