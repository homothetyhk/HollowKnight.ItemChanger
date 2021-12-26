using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Boss Essence Location which supports a hint from reading Bretta's Diary.
    /// </summary>
    public class GreyPrinceZoteLocation : DreamBossLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddLanguageEdit(new("Minor NPC", "BRETTA_DIARY_1"), AddHintToDiary);
            Events.AddLanguageEdit(new("Minor NPC", "BRETTA_DIARY_2"), AddHintToDiary);
            Events.AddLanguageEdit(new("Minor NPC", "BRETTA_DIARY_3"), AddHintToDiary);
            Events.AddLanguageEdit(new("Minor NPC", "BRETTA_DIARY_4"), AddHintToDiary);
            // Not critical, as this one shows only if Bretta has left
            Events.AddLanguageEdit(new("CP2", "BRETTA_DIARY_LEAVE"), AddHintToDiary);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveLanguageEdit(new("Minor NPC", "BRETTA_DIARY_1"), AddHintToDiary);
            Events.RemoveLanguageEdit(new("Minor NPC", "BRETTA_DIARY_2"), AddHintToDiary);
            Events.RemoveLanguageEdit(new("Minor NPC", "BRETTA_DIARY_3"), AddHintToDiary);
            Events.RemoveLanguageEdit(new("Minor NPC", "BRETTA_DIARY_4"), AddHintToDiary);
            Events.RemoveLanguageEdit(new("CP2", "BRETTA_DIARY_LEAVE"), AddHintToDiary);
        }

        private void AddHintToDiary(ref string value)
        {
            if (HintActive)
            {
                if (Placement.AllObtained()) return;

                string item = Placement.GetUIName(40);
                if (string.IsNullOrEmpty(item)) return;

                Placement.AddVisitFlag(VisitState.Previewed);
                value += $"<page>The Maiden's Treasure<br>Pondering what to gift her saviour, the damsel thought of the precious "
                    + item + " under her room. Though difficult to part with, she had nothing better with which to thank them.";
            }
        }
    }
}
