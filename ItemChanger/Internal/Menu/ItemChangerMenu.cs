using Modding;
using MenuEntry = Modding.IMenuMod.MenuEntry;

namespace ItemChanger.Internal.Menu
{
    public static class ItemChangerMenu
    {
        public readonly record struct SubpageDef(string Title, string Description, MenuEntry[] Entries);
        public static readonly List<SubpageDef> Subpages = new()
        {
            new() 
            {
                Title = "Preload Settings", 
                Description = "Changes to preload settings will not take effect until reloading the game.", 
                Entries = ItemChangerMod.GS.PreloadSettings.GetMenuEntries(),
            },
            new() 
            {
                Title = "Location Settings", 
                Description = "Changes to location settings will not affect old save files.", 
                Entries = ItemChangerMod.GS.LocationSettings.GetMenuEntries(),
            },
        };


        public static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            ModMenuScreenBuilder builder = new("ItemChangerMod", modListMenu);
            foreach (SubpageDef def in Subpages) builder.AddSubpage(def.Title, def.Description, def.Entries);
            return builder.CreateMenuScreen();
        }
    }
}
