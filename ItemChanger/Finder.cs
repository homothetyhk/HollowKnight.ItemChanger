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

            AbstractLocation[] locs = Locations.Values.ToArray();
            Locations.ExistingContainerLocation GetLocation(Locations.ObjectLocation ol)
            {
                return new Locations.ExistingContainerLocation
                {
                    name = ol.name,
                    sceneName = ol.sceneName,
                    flingType = ol.flingType,
                    containerType = Container.Shiny,
                    elevation = ol.elevation,
                    fsmName = "Shiny Control",
                    nonreplaceable = false,
                    objectName = ol.objectName,
                };
            }

            foreach (var loc in locs)
            {
                if (loc is Locations.ObjectLocation ol && loc.GetType() == typeof(Locations.ObjectLocation) && ol.objectName.StartsWith("Shiny"))
                {
                    if (ol.objectName.Contains('/') || ol.objectName.Contains('\\'))
                    {
                        ItemChangerMod.instance.LogError(ol.name);
                        throw new ArgumentException(ol.name);
                    }

                    Locations[ol.name] = GetLocation(ol);
                }
                else if (loc is Locations.DualLocation dl)
                {
                    if (dl.name.StartsWith("Boss_Geo")) continue;
                    if (dl.falseLocation is Locations.ObjectLocation fol && fol.GetType() == typeof(Locations.ObjectLocation) && fol.objectName.StartsWith("Shiny"))
                    {
                        dl.falseLocation = GetLocation(fol);
                    }
                    if (dl.trueLocation is Locations.ObjectLocation tol && tol.GetType() == typeof(Locations.ObjectLocation) && tol.objectName.StartsWith("Shiny"))
                    {
                        dl.trueLocation = GetLocation(tol);
                    }
                }
                else if (loc is Locations.ExistingContainerLocation ecl)
                {
                    if (ecl.name.StartsWith("Lore_Tablet")) continue;
                    if (ecl.containerType == Container.Chest)
                    {
                        ecl.nonreplaceable = true;
                    }
                }
            }

            var cst0 = Locations["King_Fragment"].AddTag<Tags.ChangeSceneTag>();
            cst0.changeTo = Transition.GetDreamReturn("Abyss_05");
            var cst1 = Locations["Lifeblood_Core"].AddTag<Tags.ChangeSceneTag>();
            cst1.changeTo = Transition.GetDreamReturn(SceneNames.Abyss_06_Core);

            Serialize("locations.json", Locations);
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
