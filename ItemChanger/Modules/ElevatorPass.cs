#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which implements the Elevator Pass custom skill.
    /// </summary>
    public class ElevatorPass : Module
    {
        public bool hasElevatorPass { get; set; }

        public override void Initialize()
        {
            if (!hasElevatorPass && PlayerData.instance.cityLift2) PlayerData.instance.SetBool(nameof(PlayerData.cityLift2), false);
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
        }

        public override void Unload()
        {
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
        }

        private bool SkillBoolGetOverride(string boolName, bool value) => boolName switch
        {
            nameof(hasElevatorPass) => hasElevatorPass,
            _ => value,
        };

        private bool SkillBoolSetOverride(string boolName, bool value)
        {
            switch (boolName)
            {
                case nameof(hasElevatorPass): 
                    hasElevatorPass = value;
                    PlayerData.instance.SetBool(nameof(PlayerData.cityLift1), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.cityLift2), true);
                    break;
            }
            return value;
        }
    }
}
