using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    public class SplitSuperdash : Module
    {
        public bool hasSuperdashLeft { get; set; }
        public bool hasSuperdashRight { get; set; }
        public bool hasSuperdashAny => hasSuperdashLeft ^ hasSuperdashRight || PlayerData.instance.GetBoolInternal(nameof(PlayerData.hasSuperDash));
        public MylaDeathCondition MylaDeathHandling = MylaDeathCondition.DieAfterFullSuperdash;

        public override void Initialize()
        {
            Events.AddFsmEdit(new("Equipment", "Build Equipment List"), EditInventory);
            Events.AddLanguageEdit(new("UI", "INV_NAME_SUPERDASH"), EditSuperdashName);
            Events.AddLanguageEdit(new("UI", "INV_DESC_SUPERDASH"), EditSuperdashDesc);
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
            On.DeactivateIfPlayerdataTrue.OnEnable += SetMylaDeathCondition;
            On.DeactivateIfPlayerdataFalse.OnEnable += SetMylaDeathCondition;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Equipment", "Build Equipment List"), EditInventory);
            Events.RemoveLanguageEdit(new("UI", "INV_NAME_SUPERDASH"), EditSuperdashName);
            Events.RemoveLanguageEdit(new("UI", "INV_DESC_SUPERDASH"), EditSuperdashDesc);
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
            On.DeactivateIfPlayerdataTrue.OnEnable -= SetMylaDeathCondition;
            On.DeactivateIfPlayerdataFalse.OnEnable -= SetMylaDeathCondition;
        }

        private void EditInventory(PlayMakerFSM fsm)
        {
            fsm.GetState("Super Dash").GetFirstActionOfType<PlayerDataBoolTest>().boolName.Value = nameof(hasSuperdashAny);
        }

        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(hasSuperdashLeft) => hasSuperdashLeft,
                nameof(hasSuperdashRight) => hasSuperdashRight,
                nameof(PlayerData.hasSuperDash) => value
                    || HeroController.instance.cState.onGround && (HeroController.instance.cState.facingRight ? hasSuperdashRight : hasSuperdashLeft)
                    || HeroController.instance.cState.wallSliding && (HeroController.instance.cState.facingRight ? hasSuperdashLeft : hasSuperdashRight),
                nameof(hasSuperdashAny) => hasSuperdashAny,
                _ => value,
            };
        }

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(hasSuperdashLeft):
                    hasSuperdashLeft = value;
                    goto BoolChanged;

                case nameof(hasSuperdashRight):
                    hasSuperdashRight = value;
                    goto BoolChanged;

                BoolChanged:
                    if (hasSuperdashLeft && hasSuperdashRight)
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.hasSuperDash), true);
                        PlayerData.instance.SetBool(nameof(PlayerData.canSuperDash), true);
                    }
                    break;
            }
            return value;
        }

        private void EditSuperdashName(ref string value)
        {
            if (hasSuperdashLeft && !hasSuperdashRight)
            {
                value = "Left Crystal Heart";
            }
            else if (!hasSuperdashLeft && hasSuperdashRight)
            {
                value = "Right Crystal Heart";
            }
        }

        private void EditSuperdashDesc(ref string value)
        {
            if (hasSuperdashLeft && !hasSuperdashRight)
            {
                value = "Part of the energy core of an old mining golem, fashioned around a potent crystal. The crystal's energy can be channeled to launch the bearer to the left at dangerous speeds.";
            }
            else if (!hasSuperdashLeft && hasSuperdashRight)
            {
                value = "Part of the energy core of an old mining golem, fashioned around a potent crystal. The crystal's energy can be channeled to launch the bearer to the right at dangerous speeds.";
            }
        }

        public enum MylaDeathCondition
        {
            Vanilla,
            DieAfterFullSuperdash = Vanilla,
            DieAfterEitherSuperdash,
            DieAfterLeftSuperdash,
            DieAfterRightSuperdash,
        }

        private void SetMylaDeathCondition(On.DeactivateIfPlayerdataTrue.orig_OnEnable orig, DeactivateIfPlayerdataTrue self)
        {
            if (self.gameObject.scene.name == SceneNames.Crossroads_45 && self.boolName == nameof(PlayerData.hasSuperDash))
            {
                self.boolName = MylaDeathHandling switch
                {
                    MylaDeathCondition.DieAfterFullSuperdash => nameof(PlayerData.hasSuperDash),
                    MylaDeathCondition.DieAfterRightSuperdash => nameof(hasSuperdashRight),
                    MylaDeathCondition.DieAfterLeftSuperdash => nameof(hasSuperdashLeft),
                    MylaDeathCondition.DieAfterEitherSuperdash => nameof(hasSuperdashAny),
                    _ => nameof(PlayerData.hasSuperDash),
                };
            }
            orig(self);
        }
        private void SetMylaDeathCondition(On.DeactivateIfPlayerdataFalse.orig_OnEnable orig, DeactivateIfPlayerdataFalse self)
        {
            if (self.gameObject.scene.name == SceneNames.Crossroads_45 && self.boolName == nameof(PlayerData.hasSuperDash))
            {
                self.boolName = MylaDeathHandling switch
                {
                    MylaDeathCondition.DieAfterFullSuperdash => nameof(PlayerData.hasSuperDash),
                    MylaDeathCondition.DieAfterRightSuperdash => nameof(hasSuperdashRight),
                    MylaDeathCondition.DieAfterLeftSuperdash => nameof(hasSuperdashLeft),
                    MylaDeathCondition.DieAfterEitherSuperdash => nameof(hasSuperdashAny),
                    _ => nameof(PlayerData.hasSuperDash),
                };
            }
            orig(self);
        }
    }
}