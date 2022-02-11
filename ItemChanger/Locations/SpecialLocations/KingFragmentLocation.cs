namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which supports a hint at the Kingsmould corpse in Abyss_05 and triggers a scene change to Abyss_05 when its items are obtained.
    /// </summary>
    public class KingFragmentLocation : ObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddLanguageEdit(new("Lore Tablets", "DUSK_KNIGHT_CORPSE"), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveLanguageEdit(new("Lore Tablets", "DUSK_KNIGHT_CORPSE"), OnLanguageGet);
        }

        private void OnLanguageGet(ref string value)
        {
            if (this.GetItemHintActive())
            {
                if (!Placement.AllObtained())
                {
                    string text = Placement.GetUIName();
                    value = string.Format(Language.Language.Get("DUSK_KNIGHT_CORPSE_HINT", "Fmt"), text);
                    Placement.OnPreview(text);
                }
                else
                {
                    value = Language.Language.Get("DUSK_KNIGHT_CORPSE_OBTAINED", "Lore Tablets");
                }
            }
        }
    }
}
