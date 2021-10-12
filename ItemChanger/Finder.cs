using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private static Dictionary<string, AbstractItem> Items;
        private static Dictionary<string, AbstractLocation> Locations;
        public static IEnumerable<string> ItemNames => Items.Keys;
        public static IEnumerable<string> LocationNames => Locations.Keys;

        public static AbstractItem GetItemInternal(string name)
        {
            return Items.TryGetValue(name, out AbstractItem item) ? item.Clone() : null;
        }

        public static AbstractItem GetItem(string name)
        {
            GetItemEventArgs args = new(name);
            GetItemOverride?.Invoke(args);
            if (args.Current != null) return args.Current;
            else return GetItemInternal(name);
        }
             
        public static Dictionary<string, AbstractItem> GetFullItemList()
        {
            return Items.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
        }

        public static AbstractLocation GetLocation(string name)
        {
            GetLocationEventArgs args = new(name);
            GetLocationOverride?.Invoke(args);
            if (args.Current != null) return args.Current;
            else return GetLocationInternal(name);
        }

        public static AbstractLocation GetLocationInternal(string name)
        {
            return Locations.TryGetValue(name, out AbstractLocation loc) ? loc.Clone() : null;
        }

        public static Dictionary<string, AbstractLocation> GetFullLocationList()
        {
            return Locations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
        }

        public static void Load()
        {
            JsonSerializer js = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            using (Stream s = ItemJson)
            using (StreamReader sr = new StreamReader(s))
            using (var jtr = new JsonTextReader(sr))
            {
                Items = js.Deserialize<Dictionary<string, AbstractItem>>(jtr);
            }

            using (Stream s = LocationJson)
            using (StreamReader sr = new StreamReader(s))
            using (var jtr = new JsonTextReader(sr))
            {
                Locations = js.Deserialize<Dictionary<string, AbstractLocation>>(jtr);
            }

            var locs = Locations.Values.Where(l => l.name.StartsWith("Soul_Totem")).Cast<Locations.ObjectLocation>()
                .ToArray();

            string GetItemName(Locations.ObjectLocation o)
            {
                string objName = o.objectName;
                var subtype = SoulTotemSubtype.A;
                if (objName == "Soul Totem 5")
                {
                    subtype = SoulTotemSubtype.A;
                }
                else if (objName == "Soul Totem mini_two_horned")
                {
                    subtype = SoulTotemSubtype.B;
                }
                else if (objName == "Soul Totem mini_horned")
                {
                    subtype = SoulTotemSubtype.C;
                }
                else if (objName == "Soul Totem 1")
                {
                    subtype = SoulTotemSubtype.D;
                }
                else if (objName == "Soul Totem 4")
                {
                    subtype = SoulTotemSubtype.E;
                }
                else if (objName == "Soul Totem 2")
                {
                    subtype = SoulTotemSubtype.F;
                }
                else if (objName == "Soul Totem 3")
                {
                    subtype = SoulTotemSubtype.G;
                }
                else if (objName == "Soul Totem white")
                {
                    subtype = SoulTotemSubtype.Palace;
                }
                else if (objName.StartsWith("Soul Totem white_Infinte"))
                {
                    subtype = SoulTotemSubtype.PathOfPain;
                }

                return subtype switch
                {
                    SoulTotemSubtype.PathOfPain => "Soul_Totem-Path_of_Pain",
                    _ => $"Soul_Totem-{subtype}"
                };
            }

            Serialize("pools.json", new
            {
                name = "Soul",
                path = "PoolSettings.SoulTotems",
                includeItems = locs.Select(o => GetItemName(o)).ToArray(),
                includeLocations = locs.Select(o => o.name).ToArray(),
                vanilla = locs.Select(o => new { item = GetItemName(o), location = o.name }).ToArray()
            });
        }

        internal static void Serialize(string filename, object o)
        {
            JsonSerializer js = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, filename)))
            {
                js.Serialize(sw, o);
            }
        }

        private static Stream ItemJson => typeof(Finder).Assembly.GetManifestResourceStream("ItemChanger.Resources.items.json");
        private static Stream LocationJson => typeof(Finder).Assembly.GetManifestResourceStream("ItemChanger.Resources.locations.json");
    }
}
