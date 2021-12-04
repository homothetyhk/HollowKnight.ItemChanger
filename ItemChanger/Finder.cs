using Newtonsoft.Json;


namespace ItemChanger
{
    public static class Finder
    {
        /// <summary>
        /// Invoked by Finder.GetItem. The initial arguments are the requested name, and null. If the event finishes with a non-null item, that item is returned to the requester.
        /// <br/>Otherwise, the ItemChanger internal implementation of that item is cloned and returned, if it exists. Otherwise, null is returned.
        /// </summary>
        public static event Action<GetItemEventArgs> GetItemOverride;
        /// <summary>
        /// Invoked by Finder.GetLocation. The initial arguments are the requested name, and null. If the event finishes with a non-null location, that location is returned to the requester.
        /// <br/>Otherwise, the ItemChanger internal implementation of that location is cloned and returned, if it exists. Otherwise, null is returned.
        /// </summary>
        public static event Action<GetLocationEventArgs> GetLocationOverride;
        

        private static Dictionary<string, AbstractItem>[] Items;
        private static Dictionary<string, AbstractLocation>[] Locations;

        private static readonly Dictionary<string, AbstractItem> CustomItems = new();
        private static readonly Dictionary<string, AbstractLocation> CustomLocations = new();

        public static IEnumerable<string> ItemNames => Items[0].Keys;
        public static IEnumerable<string> LocationNames => Locations[0].Keys;

        /// <summary>
        /// The most general method for looking up an item. Invokes an event to allow subscribers to modify the search result. Return value defaults to that of GetItemInternal.
        /// </summary>
        public static AbstractItem GetItem(string name)
        {
            GetItemEventArgs args = new(name);
            GetItemOverride?.Invoke(args);
            if (args.Current != null) return args.Current;
            else return GetItemInternal(name);
        }

        /// <summary>
        /// Finds the itme by name in the sheet with the requested index, and returns a clone of the item. Returns null if the item was not found.
        /// </summary>
        public static AbstractItem GetItemFromSheet(string name, int sheet)
        {
            return Items[sheet].TryGetValue(name, out AbstractItem item) ? item.Clone() : null;
        }

        /// <summary>
        /// Searches for the item by name among the requested sheets, and returns a clone of the item from the first sheet with a match. Returns null if the item was not found.
        /// </summary>
        public static AbstractItem GetItemFromSheet(string name, IEnumerable<int> sheets)
        {
            if (sheets == null) return null;
            foreach (int i in sheets)
            {
                if (Items[i].TryGetValue(name, out AbstractItem item)) return item.Clone();
            }
            return null;
        }

        /// <summary>
        /// Searches for the item by name, first in the CustomItems list, then in the list of extra sheets held by GlobalSettings, and finally in the default item sheet. Returns null if not found.
        /// </summary>
        public static AbstractItem GetItemInternal(string name)
        {
            return CustomItems.TryGetValue(name, out AbstractItem item)
                ? item.Clone()
                : GetItemFromSheet(name, ItemChangerMod.GS.ItemSettings.extraSheets)
                ?? GetItemFromSheet(name, 0);
        }

        public static Dictionary<string, AbstractItem> GetFullItemList()
        {
            return GetItemList(0);
        }

        public static Dictionary<string, AbstractItem> GetItemList(int id)
        {
            return Items[id].ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
        }

        public static Dictionary<string, AbstractItem>[] GetItemLists()
        {
            Dictionary<string, AbstractItem>[] list = new Dictionary<string, AbstractItem>[Items.Length];
            for (int i = 0; i < list.Length; i++) list[i] = GetItemList(i);
            return list;
        }

        /// <summary>
        /// The most general method for looking up a location. Invokes an event to allow subscribers to modify the search result. Return value defaults to that of GetLocationInternal.
        /// </summary>
        public static AbstractLocation GetLocation(string name)
        {
            GetLocationEventArgs args = new(name);
            GetLocationOverride?.Invoke(args);
            if (args.Current != null) return args.Current;
            else return GetLocationInternal(name);
        }

        /// <summary>
        /// Finds the location by name in the sheet with the requested index, and returns a clone of the location. Returns null if the location was not found.
        /// </summary>
        public static AbstractLocation GetLocationFromSheet(string name, int sheet)
        {
            return Locations[sheet].TryGetValue(name, out AbstractLocation loc) ? loc.Clone() : null;
        }

        /// <summary>
        /// Searches for the location by name among the requested sheets, and returns a clone of the location from the first sheet with a match. Returns null if the location was not found.
        /// </summary>
        public static AbstractLocation GetLocationFromSheet(string name, IEnumerable<int> sheets)
        {
            if (sheets == null) return null;
            foreach (int i in sheets)
            {
                if (Locations[i].TryGetValue(name, out AbstractLocation loc)) return loc.Clone();
            }
            return null;
        }

        /// <summary>
        /// Searches for the location by name, first in the CustomLocations list, then in the list of extra sheets held by GlobalSettings, and finally in the default location sheet. Returns null if not found.
        /// </summary>
        public static AbstractLocation GetLocationInternal(string name)
        {
            return CustomLocations.TryGetValue(name, out AbstractLocation loc)
                ? loc.Clone()
                : GetLocationFromSheet(name, ItemChangerMod.GS.LocationSettings.extraSheets) 
                ?? GetLocationFromSheet(name, 0);
        }

        public static Dictionary<string, AbstractLocation> GetFullLocationList()
        {
            return GetLocationList(0);
        }

        public static Dictionary<string, AbstractLocation> GetLocationList(int id)
        {
            return Locations[id].ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
        }

        public static Dictionary<string, AbstractLocation>[] GetLocationLists()
        {
            Dictionary<string, AbstractLocation>[] list = new Dictionary<string, AbstractLocation>[Locations.Length];
            for (int i = 0; i < list.Length; i++) list[i] = GetLocationList(i);
            return list;
        }

        public static void DefineCustomItem(AbstractItem item)
        {
            if (Items[0].ContainsKey(item.name)) throw new ArgumentException($"Item {item.name} is already defined by ItemChanger.");
            else if (CustomItems.ContainsKey(item.name)) throw new ArgumentException($"Item {item.name} is already defined as a custom item.");

            CustomItems.Add(item.name, item);
        }

        public static bool UndefineCustomItem(string name) => CustomItems.Remove(name);

        public static void DefineCustomLocation(AbstractLocation loc)
        {
            if (Locations[0].ContainsKey(loc.name)) throw new ArgumentException($"Location {loc.name} is already defined by ItemChanger.");
            else if (CustomLocations.ContainsKey(loc.name)) throw new ArgumentException($"Location {loc.name} is already defined as a custom location.");

            CustomLocations.Add(loc.name, loc);
        }

        public static bool UndefineCustomLocation(string name) => CustomLocations.Remove(name);

        public static void Load()
        {
            JsonSerializer js = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            System.Reflection.Assembly a = typeof(Finder).Assembly;

            Items = new Dictionary<string, AbstractItem>[itemResourcePaths.Length];
            for (int i = 0; i < itemResourcePaths.Length; i++)
            {
                using Stream s = a.GetManifestResourceStream($"ItemChanger.Resources.{itemResourcePaths[i].file}");
                using StreamReader sr = new(s);
                using JsonTextReader jtr = new(sr);
                Items[i] = js.Deserialize<Dictionary<string, AbstractItem>>(jtr);
            }

            Locations = new Dictionary<string, AbstractLocation>[locationResourcePaths.Length];
            for (int i = 0; i < locationResourcePaths.Length; i++)
            {
                using Stream s = a.GetManifestResourceStream($"ItemChanger.Resources.{locationResourcePaths[i].file}");
                using StreamReader sr = new(s);
                using JsonTextReader jtr = new(sr);
                Locations[i] = js.Deserialize<Dictionary<string, AbstractLocation>>(jtr);
            }
        }

        public static void Serialize(string filename, object o)
        {
            JsonSerializer js = new()
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            using StreamWriter sw = new(Path.Combine(Path.GetDirectoryName(typeof(Finder).Assembly.Location), filename));
            js.Serialize(sw, o);
        }

        public enum FinderItemSheets
        {
            Default = 0
        }
        private static readonly (FinderItemSheets sheet, string file)[] itemResourcePaths = new[]
        {
            (FinderItemSheets.Default, "items.json"),
        };

        public enum FinderLocationSheets
        {
            Default = 0,
            AvoidNPCItemDialogue = 1,
        }
        private static readonly (FinderLocationSheets sheet, string file)[] locationResourcePaths = new[]
        {
            (FinderLocationSheets.Default, "locations.json"),
            (FinderLocationSheets.AvoidNPCItemDialogue, "altlocations.json")
        };
    }
}
