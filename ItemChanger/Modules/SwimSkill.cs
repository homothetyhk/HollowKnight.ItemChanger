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
            Events.AddFsmEdit(new("Splash Surface", "Corpse Splash"), AddHazardSplash);
            Modding.ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            Modding.ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Surface Water Region"), EditWaterSurface);
            Events.RemoveFsmEdit(new("Splash Surface", "Corpse Splash"), AddHazardSplash);
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

            Transform splash = fsm.transform.Find("Splash Surface");
            splash.gameObject.layer = 17;

            DamageHero dh = splash.gameObject.AddComponent<DamageHero>();
            dh.damageDealt = 1;
            dh.hazardType = (int)HazardType.ACID;

            void DoActivateSwim()
            {
                dh.damageDealt = 0;
            }

            FsmState checkSwim = fsm.AddState("Check Swim");
            FsmState activateSwim = fsm.AddState("Activate Swim");
            FsmState idle = fsm.GetState("Idle");
            foreach (FsmTransition t in fsm.FsmStates.SelectMany(s => s.Transitions))
            {
                if (t.ToFsmState == idle || t.ToState == "Idle") t.SetToState(checkSwim);
            }
            checkSwim.AddTransition("SWIM GET", activateSwim);
            checkSwim.AddFirstAction(new DelegateBoolTest(() => canSwim, "SWIM GET", null));
            activateSwim.AddFirstAction(new Lambda(DoActivateSwim));
            activateSwim.AddTransition(FsmEvent.Finished, idle);
        }

        private void AddHazardSplash(PlayMakerFSM fsm)
        {
            FsmState detect = fsm.GetState("Detect");
            FsmState splash = fsm.GetState("Splash");
            FsmState checkSwim = fsm.AddState("Check Swim");

            // corpse splash seems to always be black, so let's fix that
            if (fsm.transform.parent?.gameObject?.LocateMyFSM("Surface Water Region") is PlayMakerFSM swr)
            {
                bool black = swr.FsmVariables.FindFsmBool("Black").Value;
                if (!black)
                {
                    FsmState state = swr.GetState("Blue");
                    splash.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value
                        = state.GetFirstActionOfType<SetGameObject>().gameObject.Value;
                    splash.GetFirstActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value
                        = state.GetActionsOfType<SetGameObject>()[1].gameObject.Value;
                }
            }

            Trigger2dEventLayer t = detect.GetFirstActionOfType<Trigger2dEventLayer>();
            FsmGameObject fgo = t.storeCollider;
            detect.AddLastAction(new Trigger2dEvent
            {
                collideLayer = new(""),
                collideTag = "HeroBox",
                sendEvent = FsmEvent.GetFsmEvent("CHECK"),
                storeCollider = fgo,
                trigger = t.trigger,
            });
            detect.AddTransition("CHECK", checkSwim);

            checkSwim.AddFirstAction(new DelegateBoolTest(() => !canSwim, "SPLASH", "CANCEL"));
            checkSwim.AddTransition("CANCEL", detect);
            checkSwim.AddTransition("SPLASH", splash);
        }
    }
}
