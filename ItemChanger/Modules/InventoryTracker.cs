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
        [field: JsonIgnore] public event Action<StringBuilder>? OnGenerateFocusDesc;

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

        private void EditFocusName(ref string value) => value = Language.Language.Get("TRACKER_NAME", "IC");

        private void EditFocusDesc(ref string value)
        {
            StringBuilder sb = new();
            ModuleCollection mods = ItemChangerMod.Modules;
            FocusSkill? fs = mods.Get<FocusSkill>();
            SwimSkill? ss = mods.Get<SwimSkill>();
            ElevatorPass? ep = mods.Get<ElevatorPass>();

            if (fs is not null)
            {
                if (fs.canFocus) sb.Append(Language.Language.Get("TRACKER_CAN_FOCUS", "IC"));
                else sb.Append(Language.Language.Get("TRACKER_CANNOT_FOCUS", "IC"));

                if (ss is not null) sb.Append(' ');
                else sb.AppendLine();
            }

            if (ss is not null)
            {
                if (ss.canSwim) sb.Append(Language.Language.Get("TRACKER_CAN_SWIM", "IC"));
                else sb.Append(Language.Language.Get("TRACKER_CANNOT_SWIM", "IC"));
                sb.AppendLine();
            }

            if (ep is not null)
            {
                if (ep.hasElevatorPass) sb.Append(Language.Language.Get("TRACKER_HAS_ELEVATOR_PASS", "IC"));
                else sb.Append(Language.Language.Get("TRACKER_NO_ELEVATOR_PASS", "IC"));
                sb.AppendLine();
            }

            if (!Ref.PD.GetBool(nameof(Ref.PD.hasDreamNail)))
            {
                int essence = Ref.PD.GetInt(nameof(Ref.PD.dreamOrbs));
                if (essence > 0)
                {
                    sb.AppendFormat(Language.Language.Get("TRACKER_ESSENCE", "Fmt"), essence).AppendLine();
                }
            }

            if (TrackGrimmkinFlames && PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) <= 3)
            {
                if (mods.Get<GrimmkinFlameManager>() is GrimmkinFlameManager gfm)
                {
                    sb.AppendFormat(Language.Language.Get("TRACKER_FLAMES", "Fmt"), gfm.flameBalance, gfm.cumulativeFlamesCollected).AppendLine();
                }
                else
                {
                    sb.AppendFormat(Language.Language.Get("TRACKER_FLAMES_NO_GFM", "Fmt"), PlayerData.instance.GetInt(nameof(PlayerData.flamesCollected))).AppendLine();
                } 
            }

            sb.AppendFormat(Language.Language.Get("TRACKER_GRUBS", "Fmt"), PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected))).AppendLine();

            int dreamers = Ref.PD.GetInt(nameof(PlayerData.guardiansDefeated));
            if (dreamers == 0) sb.AppendLine(Language.Language.Get("TRACKER_NO_DREAMERS", "IC"));
            else if (dreamers > 0)
            {
                sb.AppendFormat(Language.Language.Get("TRACKER_DREAMERS", "Fmt"), dreamers).AppendLine();

                bool lurien = Ref.PD.GetBool(nameof(PlayerData.lurienDefeated));
                bool monomon = Ref.PD.GetBool(nameof(PlayerData.monomonDefeated));
                bool herrah = Ref.PD.GetBool(nameof(PlayerData.hegemolDefeated));

                if (lurien) dreamers--;
                if (monomon) dreamers--;
                if (herrah) dreamers--;
                bool dupe = dreamers > 0;

                sb.AppendLine(string.Join(Language.Language.Get("COMMA_SPACE", "IC"), 
                    new (bool, string)[]
                    {
                        (lurien, Language.Language.Get("TRACKER_LURIEN", "IC")),
                        (monomon, Language.Language.Get("TRACKER_MONOMON", "IC")),
                        (herrah, Language.Language.Get("TRACKER_HERRAH", "IC")),
                        (dupe, Language.Language.Get("TRACKER_DUPLICATE_DREAMERS", "IC")),
                    }.Where(p => p.Item1).Select(p => p.Item2)));
            }
            OnGenerateFocusDesc?.Invoke(sb);

            value = sb.ToString();
        }

        private void EditNailDesc(ref string value)
        {
            SplitNail? sn = ItemChangerMod.Modules.Get<SplitNail>();

            if (sn is not null)
            {
                StringBuilder sb = new();

                string[] abilities = new[]
                {
                    (sn.canDownslash, Language.Language.Get("TRACKER_DOWN", "IC")),
                    (sn.canUpslash, Language.Language.Get("TRACKER_UP", "IC")),
                    (sn.canSideslashLeft, Language.Language.Get("TRACKER_LEFT", "IC")),
                    (sn.canSideslashRight, Language.Language.Get("TRACKER_RIGHT", "IC")),
                }.Where(p => p.Item1).Select(p => p.Item2).ToArray();

                sb.Append("<br><br>");
                if (abilities.Length > 0)
                {
                    sb.AppendFormat(Language.Language.Get("TRACKER_CAN_NAIL", "Fmt"), string.Join(Language.Language.Get("COMMA_SPACE", "IC"), abilities));
                }
                else
                {
                    sb.Append(Language.Language.Get("TRACKER_CANNOT_NAIL", "IC"));
                }

                value += sb.ToString();
            }
        }
    }
}
