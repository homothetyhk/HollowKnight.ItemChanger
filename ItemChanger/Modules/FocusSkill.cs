using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
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
            Events.AddFsmEdit(SceneNames.Room_Final_Boss_Core, new("Boss Corpse", "Corpse"), RemoveGateWithNoFocus);
            Events.AddSceneChangeEdit(SceneNames.Room_Final_Boss_Atrium, RemoveTHKDeathState);
        }

        public override void Unload()
        {
            On.HeroController.CanFocus -= OverrideCanFocus;
            Modding.ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
            Events.RemoveFsmEdit(SceneNames.Room_Final_Boss_Core, new("Boss Corpse", "Corpse"), RemoveGateWithNoFocus);
            Events.RemoveSceneChangeEdit(SceneNames.Room_Final_Boss_Atrium, RemoveTHKDeathState);
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

        private void RemoveGateWithNoFocus(PlayMakerFSM fsm)
        {
            fsm.GetState("Burst").AddLastAction(new Lambda(() =>
            {
                if (!canFocus)
                {
                    UObject.Destroy(fsm.gameObject.scene.FindGameObjectByName("Gate"));
                }
            }));
        }

        private void RemoveTHKDeathState(Scene scene)
        {
            if (GameManager.instance.entryGateName != "right1") return;

            FsmVariables.GlobalVariables.FindFsmGameObject("HUD Canvas").Value.LocateMyFSM("Slide Out").SendEvent("IN");
            PlayerData.instance.SetBool(nameof(PlayerData.disablePause), false);
            FsmVariables.GlobalVariables.FindFsmGameObject("CameraParent").Value.LocateMyFSM("CameraShake").FsmVariables.FindFsmBool("RumblingMed").Value = false;
            FsmVariables.GlobalVariables.FindFsmGameObject("CameraParent").Value.LocateMyFSM("CameraShake").FsmVariables.FindFsmBool("RumblingSmall").Value = false;
            HeroController.SilentInstance.spellControl.FsmVariables.FindFsmBool("Dream Focus").Value = false;
        }
    }
}
