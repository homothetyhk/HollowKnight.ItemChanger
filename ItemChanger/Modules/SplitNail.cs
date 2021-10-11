using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalEnums;
using UnityEngine;
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

        public override void Initialize()
        {
            On.HeroController.CanAttack += ModifyNail;
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
        }

        public override void Unload()
        {
            On.HeroController.CanAttack -= ModifyNail;
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
        }

        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(canSideslashLeft) => canSideslashLeft,
                nameof(canSideslashRight) => canSideslashRight,
                nameof(canUpslash) => canUpslash,
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
            }
            return value;
        }

        private bool ModifyNail(On.HeroController.orig_CanAttack orig, HeroController self)
        {
            return GetAttackDirection(self) switch
            {
                Direction.upward => orig(self) && canUpslash,
                Direction.leftward => orig(self) && canSideslashLeft,
                Direction.rightward => orig(self) && canSideslashRight,
                Direction.downward => orig(self),
                _ => orig(self),
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
