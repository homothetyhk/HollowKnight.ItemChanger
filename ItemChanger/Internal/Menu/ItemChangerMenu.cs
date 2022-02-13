using Modding;
using MenuEntry = Modding.IMenuMod.MenuEntry;

namespace ItemChanger.Internal.Menu
{
    public static class ItemChangerMenu
    {
        public readonly record struct SubpageDef(string TitleKey, string DescriptionKey, MenuEntry[] Entries);
        public static readonly List<SubpageDef> Subpages = new()
        {
            new() 
            {
                TitleKey = "PRELOAD_SETTINGS_NAME", 
                DescriptionKey = "PRELOAD_SETTINGS_DESC", 
                Entries = ItemChangerMod.GS.PreloadSettings.GetMenuEntries(),
            },
            new() 
            {
                TitleKey = "LOCATION_SETTINGS_NAME", 
                DescriptionKey = "LOCATION_SETTINGS_DESC", 
                Entries = ItemChangerMod.GS.LocationSettings.GetMenuEntries(),
            },
        };


        public static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            ModMenuScreenBuilder builder = new(LanguageStringManager.GetICString("ITEMCHANGERMOD"), modListMenu);
            foreach (SubpageDef def in Subpages)
            {
                builder.AddSubpage(LanguageStringManager.GetICString(def.TitleKey), LanguageStringManager.GetICString(def.DescriptionKey), def.Entries);
            }

            return builder.CreateMenuScreen();
        }
    }
}
