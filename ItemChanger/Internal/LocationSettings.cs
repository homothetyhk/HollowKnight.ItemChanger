using MenuEntry = Modding.IMenuMod.MenuEntry;

namespace ItemChanger.Internal
{
    public class LocationSettings
    {
        public readonly record struct LocationSheetSetting(string NameKey, int SheetIndex);
        public static readonly LocationSheetSetting[] Settings = new LocationSheetSetting[]
        {
            new("AVOID_NPC_ITEM_DIALOGUE_NAME", (int)Finder.FinderLocationSheets.AvoidNPCItemDialogue),
            new("AVOID_BLUGGSACS_NAME", (int)Finder.FinderLocationSheets.AvoidBluggsacs),
            new("RETAIN_TABLETS_ON_REPLACE_NAME", (int)Finder.FinderLocationSheets.RetainTabletsOnReplace),
        };

        public List<int> extraSheets = new();

        public void AddSheet(int id)
        {
            if (extraSheets.Contains(id)) return;
            extraSheets.Add(id);
        }

        public void RemoveSheet(int id)
        {
            extraSheets.Remove(id);
        }

        public void ToggleSheet(bool value, int id)
        {
            if (value) AddSheet(id);
            else RemoveSheet(id);
        }

        public bool HasSheet(int id)
        {
            return extraSheets.Contains(id);
        }

        public MenuEntry[] GetMenuEntries()
        {
            string[] bools = new string[] { LanguageStringManager.GetICString("FALSE"), LanguageStringManager.GetICString("TRUE") };
            return Settings.Select(s => new MenuEntry(
                name: LanguageStringManager.GetICString(s.NameKey),
                values: bools,
                description: string.Empty,
                saver: j => ToggleSheet(j == 1, s.SheetIndex),
                loader: () => HasSheet(s.SheetIndex) ? 1 : 0)
            ).ToArray();
        }
    }
}
