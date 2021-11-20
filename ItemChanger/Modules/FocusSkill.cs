#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which implements the Focus custom skill.
    /// </summary>
    public class FocusSkill : Module
    {
        public bool canFocus { get; set; }

        public override void Initialize()
        {
            On.HeroController.CanFocus += OverrideCanFocus;
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
        }

        public override void Unload()
        {
            On.HeroController.CanFocus -= OverrideCanFocus;
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
        }

        private bool SkillBoolGetOverride(string boolName, bool value)
        {
            return boolName switch
            {
                nameof(canFocus) => canFocus,
                _ => value,
            };
        }

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(canFocus):
                    canFocus = value;
                    break;
            }
            return value;
        }

        private bool OverrideCanFocus(On.HeroController.orig_CanFocus orig, HeroController self)
        {
            return orig(self) && canFocus;
        }
    }
}
