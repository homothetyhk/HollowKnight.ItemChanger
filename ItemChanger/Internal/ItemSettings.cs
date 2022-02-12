using MenuEntry = Modding.IMenuMod.MenuEntry;

namespace ItemChanger.Internal
{
    public class ItemSettings
    {
        public readonly record struct ItemSheetSetting(string NameKey, int SheetIndex);
        public static readonly ItemSheetSetting[] Settings = new ItemSheetSetting[]
        {
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
