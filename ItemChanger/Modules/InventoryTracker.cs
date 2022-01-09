using System.Text;
using ItemChanger.Internal;
using Newtonsoft.Json;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which adds extra information to the inventory for grubs, dreamers, and similar things that cannot otherwise be checked.
    /// </summary>
    [DefaultModule]
    public class InventoryTracker : Module
    {
        public bool TrackGrimmkinFlames = true;
        [field: JsonIgnore] public event Action<StringBuilder> OnGenerateFocusDesc;

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
                if (fs.canFocus) sb.Append("You can focus.");
                else sb.Append("You cannot focus.");

                if (ss != null) sb.Append(' ');
                else sb.AppendLine();
            }

            if (ss != null)
            {
                if (ss.canSwim) sb.Append("You can swim.");
                else sb.Append("You cannot swim.");
                sb.AppendLine();
            }

            if (!Ref.PD.GetBool(nameof(Ref.PD.hasDreamNail)))
            {
                int essence = Ref.PD.GetInt(nameof(Ref.PD.dreamOrbs));
                if (essence > 0) sb.AppendLine($"You have {essence} essence.");
            }

            if (TrackGrimmkinFlames && PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) <= 3)
            {
                if (mods.Get<GrimmkinFlameManager>() is GrimmkinFlameManager gfm)
                {
                    sb.AppendLine($"You have {gfm.flameBalance} unspent Grimmkin Flame(s). ({gfm.cumulativeFlamesCollected} total)");
                }
                else
                {
                    sb.AppendLine($"You have {PlayerData.instance.GetInt(nameof(PlayerData.flamesCollected))} unspent Grimmkin Flame(s).");
                } 
            }

            sb.AppendLine($"You've rescued {Ref.PD.GetInt(nameof(PlayerData.grubsCollected))} grub(s) so far!");
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
            sb.AppendLine();

            OnGenerateFocusDesc?.Invoke(sb);

            value = sb.ToString();
        }

        private void EditNailDesc(ref string value)
        {
            SplitNail sn = ItemChangerMod.Modules.Get<SplitNail>();

            if (sn != null)
            {
                StringBuilder sb = new();

                string[] abilities = new[]
                {
                    (sn.canDownslash, "down"),
                    (sn.canUpslash, "up"),
                    (sn.canSideslashLeft, "left"),
                    (sn.canSideslashRight, "right"),
                }.Where(p => p.Item1).Select(p => p.Item2).ToArray();

                if (abilities.Length > 0)
                {
                    sb.Append("<br><br>Can be swung ");
                    sb.Append(string.Join(", ", abilities));
                    sb.Append('.');
                }
                else
                {
                    sb.Append("<br><br>Cannot be swung.");
                }

                value += sb.ToString();
            }
        }
    }
}
