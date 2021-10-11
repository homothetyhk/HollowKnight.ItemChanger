using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Internal;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which adds extra information to the inventory for grubs, dreamers, and similar things that cannot otherwise be checked.
    /// </summary>
    [DefaultModule]
    public class InventoryTracker : Module
    {
        public override void Initialize()
        {
            Events.AddLanguageEdit(new("UI", "INV_NAME_SPELL_FOCUS"), EditFocusName);
            Events.AddLanguageEdit(new("UI", "INV_DESC_SPELL_FOCUS"), EditFocusDesc);
            for (int i = 1; i <= 5; i++) Events.AddLanguageEdit(new("UI", "INV_DESC_NAIL" + i), EditNailDesc);
        }

        public override void Unload()
        {
            Events.RemoveLanguageEdit(new("UI", "INV_NAME_SPELL_FOCUS"), EditFocusName);
            Events.RemoveLanguageEdit(new("UI", "INV_DESC_SPELL_FOCUS"), EditFocusDesc);
            for (int i = 1; i <= 5; i++) Events.RemoveLanguageEdit(new("UI", "INV_DESC_NAIL" + i), EditNailDesc);
        }

        private void EditFocusName(ref string value) => value = "Tracker";

        private void EditFocusDesc(ref string value)
        {
            StringBuilder sb = new();
            ModuleCollection mods = ItemChangerMod.Modules;
            FocusSkill fs = mods.Get<FocusSkill>();
            SwimSkill ss = mods.Get<SwimSkill>();

            if (fs != null)
            {
                if (fs.canFocus) sb.AppendLine("You can focus.");
                else sb.AppendLine("You cannot focus.");
            }

            if (ss != null)
            {
                if (ss.canSwim) sb.AppendLine("You can swim.");
                else sb.AppendLine("You cannot swim.");
            }

            if (!Ref.PD.GetBool(nameof(Ref.PD.hasDreamNail)))
            {
                int essence = Ref.PD.GetInt(nameof(Ref.PD.dreamOrbs));
                if (essence > 0) sb.AppendLine($"You have {essence} essence.");
            }

            if (PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) <= 3)
            {
                sb.AppendLine($"You have {Ref.PD.flamesCollected} unspent Flames.");
            }

            sb.AppendLine($"You've rescued {PlayerData.instance.grubsCollected} grub(s) so far!");
            int dreamers = Ref.PD.GetInt(nameof(PlayerData.guardiansDefeated));
            sb.Append($"You've found {dreamers} dreamer(s)");
            if (dreamers > 0)
            {
                sb.AppendLine(", including:");
                bool lurien = Ref.PD.GetBool(nameof(PlayerData.lurienDefeated));
                bool monomon = Ref.PD.GetBool(nameof(PlayerData.monomonDefeated));
                bool herrah = Ref.PD.GetBool(nameof(PlayerData.hegemolDefeated));

                if (lurien)
                {
                    sb.Append("Lurien, ");
                    dreamers--;
                }
                if (monomon)
                {
                    sb.Append("Monomon, ");
                    dreamers--;
                }
                if (herrah)
                {
                    sb.Append("Herrah, ");
                    dreamers--;
                }
                if (dreamers > 0)
                {
                    sb.Append("Duplicate Dreamer(s)");
                }
            }

            value = sb.ToString();
        }

        private void EditNailDesc(ref string value)
        {
            SplitNail sn = ItemChangerMod.Modules.Get<SplitNail>();

            if (sn != null)
            {
                StringBuilder sb = new();
                sb.Append("<br><br>Can be swung down");
                if (sn.canUpslash) sb.Append(", up");
                if (sn.canSideslashLeft) sb.Append(", left");
                if (sn.canSideslashRight) sb.Append(", right");
                sb.Append('.');
                value += sb.ToString();
            }
        }
    }
}
