using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using Modding;

namespace ItemChanger.Modules
{
    /// <summary>
    /// A module which allows toggling between Vengeful Spirit and Shade Soul in the inventory after Shade Soul is obtained.
    /// Has optional settings to only allow a deferred one-way upgrade from Vengeful Spirit to Shade Soul.
    /// </summary>
    public class ToggleableFireballUpgrade : Module
    {
        public bool isDeferred;
        public bool onewayToggle;
        private FsmStateAction? action;

        public override void Initialize()
        {
            ModHooks.GetPlayerIntHook += GetPlayerIntHook;
            Events.AddLanguageEdit(new("INV_DESC_SPELL_FIREBALL1"), EditFireball1Description);
            Events.AddLanguageEdit(new("INV_DESC_SPELL_FIREBALL2"), EditFireball2Description);
            Events.AddFsmEdit(new("Inv", "UI Inventory"), AllowDeferToggle);
        }

        public override void Unload()
        {
            ModHooks.GetPlayerIntHook -= GetPlayerIntHook;
            Events.RemoveLanguageEdit(new("INV_DESC_SPELL_FIREBALL1"), EditFireball1Description);
            Events.RemoveLanguageEdit(new("INV_DESC_SPELL_FIREBALL2"), EditFireball2Description);
            Events.RemoveFsmEdit(new("Inv", "UI Inventory"), AllowDeferToggle);
        }

        private int GetPlayerIntHook(string name, int orig)
        {
            if (name == nameof(PlayerData.fireballLevel)) return isDeferred ? Math.Min(1, orig) : orig;
            return orig;
        }

        public void Toggle()
        {
            if (PlayerData.instance.GetIntInternal(nameof(PlayerData.fireballLevel)) < 2) return;
            if (isDeferred) isDeferred = false;
            else if (!onewayToggle) isDeferred = true;
        }

        private void AllowDeferToggle(PlayMakerFSM fsm)
        {
            FsmState fireball = fsm.GetState("Fireball");
            if (fireball.Actions.Any(a => ReferenceEquals(a, action))) return;

            bool isPressed = false;
            fsm.GetState("Fireball").AddLastAction(action = new LambdaEveryFrame(ListenForAttack));
            void ListenForAttack()
            {
                if (!isPressed && InputHandler.Instance.inputActions.attack)
                {
                    isPressed = true;
                    Toggle();
                    PlayMakerFSM updateText = fsm.gameObject.LocateMyFSM("Update Text");
                    int fireballLevel = PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel));

                    updateText.FsmVariables.GetFsmString("Convo Name").Value = "INV_NAME_SPELL_FIREBALL" + fireballLevel;
                    updateText.FsmVariables.GetFsmString("Convo Desc").Value = "INV_DESC_SPELL_FIREBALL" + fireballLevel;
                    fsm.FsmVariables.FindFsmGameObject("Spell Fireball").Value.LocateMyFSM("Check Active").SetState("Appear?");

                    updateText.SendEvent("UPDATE TEXT");
                }
                else if (isPressed && !InputHandler.Instance.inputActions.attack) isPressed = false;
            }
        }

        private void EditFireball1Description(ref string value)
        {
            if (isDeferred && PlayerData.instance.GetIntInternal(nameof(PlayerData.fireballLevel)) > 1)
            {
                value += "<br><br>" + Language.Language.Get("FIREBALL1_TOGGLE", "IC");
            }
        }

        private void EditFireball2Description(ref string value)
        {
            if (!isDeferred && !onewayToggle)
            {
                value += "<br><br>" + Language.Language.Get("FIREBALL2_TOGGLE", "IC");
            }
        }

    }
}
