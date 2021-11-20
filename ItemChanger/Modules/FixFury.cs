using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which makes Fury of the Fallen compatible with max hp 1.
    /// </summary>
    [DefaultModule]
    public class FixFury : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(new("Charm Effects", "Fury"), HookFury);
            Modding.ModHooks.SetPlayerBoolHook += PlayerDataBoolListener;
            On.HeroController.MaxHealth += EnableFuryOnBench;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Charm Effects", "Fury"), HookFury);
            Modding.ModHooks.SetPlayerBoolHook -= PlayerDataBoolListener;
            On.HeroController.MaxHealth -= EnableFuryOnBench;
        }

        private bool PlayerDataBoolListener(string boolName, bool value)
        {
            PlayerData pd = PlayerData.instance;

            if (boolName == nameof(PlayerData.equippedCharm_6))
            {
                if (value)
                {
                    if (pd.health == 1 && !pd.GetBool(nameof(PlayerData.equippedCharm_27)))
                    {
                        EnableFury();
                    }
                }
                else
                {
                    DisableFury();
                }
            }
            else if (boolName == nameof(PlayerData.equippedCharm_23))
            {
                if (value)
                {
                    DisableFury();
                }
                else if (pd.maxHealthBase == 1 && pd.GetBool(nameof(PlayerData.equippedCharm_6)) && !pd.GetBool(nameof(PlayerData.equippedCharm_27)))
                {
                    EnableFury();
                }
            }
            else if (boolName == nameof(PlayerData.equippedCharm_27))
            {
                if (value)
                {
                    DisableFury();
                }
                // This set happens before the health property is set back to its normal
                // without-Joni's value, so we can't check that. Instead check that the player
                // has 1 base mask and doesn't have Unbreakable Heart on.
                else if (pd.maxHealthBase == 1 && pd.GetBool(nameof(PlayerData.equippedCharm_6)) && !pd.GetBool(nameof(PlayerData.equippedCharm_23)))
                {
                    EnableFury();
                }
            }

            return value;
        }

        // Add some transitions to the Fury FSM so we can turn Fury on or off manually
        // when Cursed Masks are on.
        private void HookFury(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Fury" && fsm.gameObject.name == "Charm Effects")
            {
                fsm.GetState("Idle").AddTransition("ENABLE FURY", "Activate");
                fsm.GetState("Activate").AddTransition("DISABLE FURY", "Deactivate");
                fsm.GetState("Stay Furied").AddTransition("DISABLE FURY", "Deactivate");
            }
        }


        private void EnableFuryOnBench(On.HeroController.orig_MaxHealth orig, HeroController self)
        {
            orig(self);
            // Enable Fury, if appropriate, when sitting on a bench, benchwarping, or
            // loading into a hardsave from the menu.
            if (PlayerData.instance.GetInt(nameof(PlayerData.health)) == 1 
                && PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_6)) 
                && !PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_27)))
            {
                EnableFury();
            }
        }

        private static void EnableFury() { PlayMakerFSM.BroadcastEvent("ENABLE FURY"); }
        private static void DisableFury() { PlayMakerFSM.BroadcastEvent("DISABLE FURY"); }
    }
}
