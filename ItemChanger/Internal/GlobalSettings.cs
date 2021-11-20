using Modding;

namespace ItemChanger.Internal
{
    public class GlobalSettings
    {
        public PreloadSettings PreloadSettings = new();

        internal void AddEntries(List<IMenuMod.MenuEntry> entries)
        {
            PreloadSettings.AddEntries(entries);
        }
    }
}
