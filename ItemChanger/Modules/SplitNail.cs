using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which implements the split nail custom skills.
    /// </summary>
    public class SplitNail : Module
    {
        public bool canSideslashLeft { get; set; }
        public bool canSideslashRight { get; set; }
        public bool canUpslash { get; set; }
        public bool canDownslash { get; set; }

        public override void Initialize()
        {
            On.HeroController.CanAttack += ModifyNail;
            On.HeroController.DoAttack += PreventAttack;
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
            Events.AddFsmEdit(new("Knight", "Dream Nail"), FixDreamNailAnim);
        }

        public override void Unload()
        {
            On.HeroController.CanAttack -= ModifyNail;
            On.HeroController.DoAttack -= PreventAttack;
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
            Events.RemoveFsmEdit(new("Knight", "Dream Nail"), FixDreamNailAnim);
        }

        private void PreventAttack(On.HeroController.orig_DoAttack orig, HeroController self)
        {
            if (CanAttackInAttackDirection(self)) orig(self);
        }
        private void FixDreamNailAnim(PlayMakerFSM fsm)
        {
            FsmState cancelable = fsm.GetState("Cancelable");
            FsmState cancelableDash = fsm.GetState("Cancelable Dash");
            
            void attackIfAble()
            {
                if (InputHandler.Instance.inputActions.attack.WasPressed && CanAttackInAttackDirection(HeroController.instance))
                {
                    fsm.Fsm.Event("ATTACK CANCEL");
                }
            }

            cancelable.Actions[6] = new LambdaEveryFrame(attackIfAble);
            cancelableDash.Actions[6] = new LambdaEveryFrame(attackIfAble);
        }
        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(canSideslashLeft) => canSideslashLeft,
                nameof(canSideslashRight) => canSideslashRight,
                nameof(canUpslash) => canUpslash,
                nameof(canDownslash) => canDownslash,
                _ => value,
            };
        }

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(canSideslashLeft):
                    canSideslashLeft = value;
                    break;
                case nameof(canSideslashRight):
                    canSideslashRight = value;
                    break;
                case nameof(canUpslash):
                    canUpslash = value;
                    break;
                case nameof(canDownslash):
                    canDownslash = value;
                    break;
            }
            return value;
        }

        private bool ModifyNail(On.HeroController.orig_CanAttack orig, HeroController self)
        {
            return orig(self) && CanAttackInAttackDirection(self);
        }

        private bool CanAttackInAttackDirection(HeroController hc)
        {
            return GetAttackDirection(hc) switch
            {
                Direction.upward => canUpslash,
                Direction.leftward => canSideslashLeft,
                Direction.rightward => canSideslashRight,
                Direction.downward => canDownslash,
                _ => true,
            };
        }

        // This function copies the code in HeroController.DoAttack to determine the attack direction, with an
        // additional check if the player is wallsliding (because we want to treat a wallslash as a normal slash)
        private static Direction GetAttackDirection(HeroController hc)
        {
            if (hc.wallSlidingL)
            {
                return Direction.rightward;
            }
            else if (hc.wallSlidingR)
            {
                return Direction.leftward;
            }

            if (hc.vertical_input > Mathf.Epsilon)
            {
                return Direction.upward;
            }
            else if (hc.vertical_input < -Mathf.Epsilon)
            {
                if (hc.hero_state != ActorStates.idle && hc.hero_state != ActorStates.running)
                {
                    return Direction.downward;
                }
                else
                {
                    return hc.cState.facingRight ? Direction.rightward : Direction.leftward;
                }
            }
            else
            {
                return hc.cState.facingRight ? Direction.rightward : Direction.leftward;
            }
        }

        private enum Direction
        {
            upward,
            leftward,
            rightward,
            downward
        }
    }
}
