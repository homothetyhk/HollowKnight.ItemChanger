using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;
using SereCore;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using GlobalEnums;

namespace ItemChanger
{
    public static class CustomSkillManager
    {
        public static void Hook()
        {
            UnHook();
            ModHooks.Instance.GetPlayerBoolHook += SkillBoolGetOverride;
            ModHooks.Instance.SetPlayerBoolHook += SkillBoolSetOverride;
            On.PlayMakerFSM.OnEnable += ModifyFsm;
            On.HeroController.CanFocus += ModifyFocus;
            On.HeroController.CanDash += ModifyDash;
            On.HeroController.CanAttack += ModifyNail;
            CustomSkills.AfterSetBool += OnSetCustomSkill;
        }

        public static void UnHook()
        {
            ModHooks.Instance.GetPlayerBoolHook -= SkillBoolGetOverride;
            ModHooks.Instance.SetPlayerBoolHook -= SkillBoolSetOverride;
            On.PlayMakerFSM.OnEnable -= ModifyFsm;
            On.HeroController.CanFocus -= ModifyFocus;
            On.HeroController.CanDash -= ModifyDash;
            On.HeroController.CanAttack -= ModifyNail;
        }

        // TODO: Picking up Mothwing Cloak should give both dashes and render split cloaks redundant
        private static void OnSetCustomSkill(string name, bool value)
        {
            switch (name)
            {
                default:
                    break;
            }
        }

        private static void ModifyFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            switch (self.FsmName)
            {
                case "Build Equipment List" when self.gameObject.name == "Equipment":
                    ShowSkillsInInventory(self);
                    break;
                case "Surface Water Region":
                    ModifySwim(self);
                    break;
            }
        }


        private static bool SkillBoolGetOverride(string boolName)
        {
            switch (boolName)
            {
                // Split Dash Overrides
                case nameof(PlayerData.canDash):
                    return (Ref.SKILLS.canDashLeft ^ Ref.SKILLS.canDashRight) || Ref.PD.GetBoolInternal(nameof(PlayerData.canDash));
                case "hasDashAny":
                    return (Ref.SKILLS.canDashLeft ^ Ref.SKILLS.canDashRight) || Ref.PD.GetBoolInternal(nameof(PlayerData.hasDash));

                // Split Claw Overrides
                case nameof(CustomSkills.hasWalljumpLeft):
                    return Ref.SKILLS.hasWalljumpLeft;
                case nameof(CustomSkills.hasWalljumpRight):
                    return Ref.SKILLS.hasWalljumpRight;
                case nameof(PlayerData.hasWalljump):
                    if (Ref.HC.touchingWallL && Ref.SKILLS.hasWalljumpLeft && !Ref.SKILLS.hasWalljumpRight)
                    {
                        return true;
                    }
                    else if (Ref.HC.touchingWallR && Ref.SKILLS.hasWalljumpRight && !Ref.SKILLS.hasWalljumpLeft)
                    {
                        return true;
                    }
                    break;
                case "hasWalljumpAny":
                    return (Ref.SKILLS.hasWalljumpLeft ^ Ref.SKILLS.hasWalljumpRight) || Ref.PD.GetBoolInternal(nameof(PlayerData.hasWalljump));
            }
            return Ref.PD.GetBoolInternal(boolName);
        }

        private static void SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                // bools for left and right cloak
                case nameof(CustomSkills.canDashLeft):
                    // Give the player shadowdash if they already have that dash direction
                    if (Ref.SKILLS.canDashLeft && value)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasShadowDash), true);
                    }
                    // Otherwise, let the player dash in that direction
                    else
                    {
                        Ref.SKILLS.canDashLeft = value;
                    }
                    if (Ref.SKILLS.canDashLeft && Ref.SKILLS.canDashRight)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasDash), true);
                    }
                    break;
                case nameof(CustomSkills.canDashRight):
                    if (Ref.SKILLS.canDashRight && value)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasShadowDash), true);
                    }
                    else
                    {
                        Ref.SKILLS.canDashRight = value;
                    }
                    if (Ref.SKILLS.canDashLeft && Ref.SKILLS.canDashRight)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasDash), true);
                    }
                    break;
                // bools for left and right claw
                // If the player has one piece and gets the other, then we give them the full mantis claw. This allows the split claw to work with other mods more easily, 
                // unless of course they have only one piece.
                case nameof(CustomSkills.hasWalljumpLeft):
                    Ref.SKILLS.hasWalljumpLeft = value;
                    if (value && Ref.SKILLS.hasWalljumpRight)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasWalljump), true);
                    }
                    break;
                case nameof(CustomSkills.hasWalljumpRight):
                    Ref.SKILLS.hasWalljumpRight = value;
                    if (value && Ref.SKILLS.hasWalljumpLeft)
                    {
                        Ref.PD.SetBool(nameof(PlayerData.hasWalljump), true);
                    }
                    break;
            }
            // Send the set through to the actual set
            Ref.PD.SetBoolInternal(boolName, value);
        }

        private static void ShowSkillsInInventory(PlayMakerFSM self)
        {
            if (self.FsmName == "Build Equipment List" && self.gameObject.name == "Equipment")
            {
                self.GetState("Walljump").GetActionOfType<PlayerDataBoolTest>().boolName.Value = "hasWalljumpAny";

                PlayerDataBoolTest[] dashChecks = self.GetState("Dash").GetActionsOfType<PlayerDataBoolTest>();
                dashChecks[0].boolName.Value = "hasDashAny";
            }
        }

        private static bool ModifyFocus(On.HeroController.orig_CanFocus orig, HeroController self)
        {
            return orig(self) && Ref.SKILLS.canFocus;
        }

        private static void ModifySwim(PlayMakerFSM self)
        {
            if (self.gameObject.LocateFSM("Acid Armour Check") != null) return; // acid

            FsmState splash = self.GetState("Big Splash?");
            FsmStateAction acidDeath = new FsmStateActions.Lambda(() =>
            {
                if (!Ref.SKILLS.canSwim)
                {
                    // this is actually the spike death despite the enum, because the acid death splashes green stuff
                    HeroController.instance.TakeDamage(self.gameObject, CollisionSide.other, 1, (int)HazardType.ACID);
                    PlayMakerFSM.BroadcastEvent("SWIM DEATH");
                }
            });

            splash.AddFirstAction(acidDeath);
            splash.AddTransition("SWIM DEATH", "Idle");
        }

        private static bool ModifyDash(On.HeroController.orig_CanDash orig, HeroController self)
        {
            // Only disable dash in a direction if they have it in the other direction. If they have both or neither dash
            // direction, then it will be handled by the original function.
            // We don't need to check if Split Cloak is active, because we only change the output if the player has exactly one cloak piece
            switch (GetDashDirection(self))
            {
                default:
                    return orig(self);
                case Direction.leftward:
                    return orig(self) && (!Ref.SKILLS.canDashRight || Ref.SKILLS.canDashLeft);
                case Direction.rightward:
                    return orig(self) && (!Ref.SKILLS.canDashLeft || Ref.SKILLS.canDashRight);
                case Direction.downward:
                    return orig(self);
            }
        }
        private static Direction GetDashDirection(HeroController hc)
        {
            InputHandler input = ReflectionHelper.GetAttr<HeroController, InputHandler>(hc, "inputHandler");
            if (!hc.cState.onGround && input.inputActions.down.IsPressed && hc.playerData.GetBool("equippedCharm_31")
                    && !(input.inputActions.left.IsPressed || input.inputActions.right.IsPressed))
            {
                return Direction.downward;
            }
            if (hc.wallSlidingL) return Direction.rightward;
            else if (hc.wallSlidingR) return Direction.leftward;
            else if (input.inputActions.right.IsPressed) return Direction.rightward;
            else if (input.inputActions.left.IsPressed) return Direction.leftward;
            else if (hc.cState.facingRight) return Direction.rightward;
            else return Direction.leftward;
        }


        private static bool ModifyNail(On.HeroController.orig_CanAttack orig, HeroController self)
        {
            switch (GetAttackDirection(self))
            {
                default:
                    return orig(self);

                case Direction.upward:
                    return orig(self) && Ref.SKILLS.canUpslash;
                case Direction.leftward:
                    return orig(self) && Ref.SKILLS.canSideslashLeft;
                case Direction.rightward:
                    return orig(self) && Ref.SKILLS.canSideslashRight;
                case Direction.downward:
                    return orig(self);
            }
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
                if (hc.hero_state != GlobalEnums.ActorStates.idle && hc.hero_state != GlobalEnums.ActorStates.running)
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