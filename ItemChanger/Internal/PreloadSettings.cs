using System.Runtime.CompilerServices;
using ItemChanger.Extensions;
using Modding;
using Newtonsoft.Json;

namespace ItemChanger.Internal
{
    public enum PreloadLevel
    {
        Full,
        Reduced,
        None,
    }

    public class PreloadSettings
    {
        [JsonProperty] private readonly Dictionary<string, PreloadLevel> PreloadLevels;
        [JsonIgnore] public PreloadLevel PreloadGeoRocks { get => Get(); set => Set(value); }
        [JsonIgnore] public PreloadLevel PreloadSoulTotems { get => Get(); set => Set(value); }
        [JsonIgnore] public PreloadLevel PreloadGrub { get => Get(); set => Set(value); }
        [JsonIgnore] public PreloadLevel PreloadMimic { get => Get(); set => Set(value); }
        //[JsonIgnore] public PreloadLevel PreloadBluggsac { get => Get(); set => Set(value); }

        public PreloadSettings()
        {
            PreloadLevels = new()
            {
                { nameof(PreloadGeoRocks), PreloadLevel.Full },
                { nameof(PreloadSoulTotems), PreloadLevel.Full },
                { nameof(PreloadGrub), PreloadLevel.Full },
                { nameof(PreloadMimic), PreloadLevel.Full },
                //{ nameof(PreloadBluggsac), PreloadLevel.Full },
            };
        }
        [JsonConstructor] public PreloadSettings(Dictionary<string, PreloadLevel> PreloadLevels) => this.PreloadLevels = PreloadLevels;

        private PreloadLevel Get([CallerMemberName] string caller = null)
        {
            if (_preloadOverrides[caller] is PreloadLevel pl) return pl;
            else return PreloadLevels[caller];
        }

        private void Set(PreloadLevel value, [CallerMemberName] string caller = null) => PreloadLevels[caller] = value;


        private static Dictionary<string, PreloadLevel?> _preloadOverrides = new()
        {
            { nameof(PreloadGeoRocks), null },
            { nameof(PreloadSoulTotems), null },
            { nameof(PreloadGrub), null },
            { nameof(PreloadMimic), null },
            //{ nameof(PreloadBluggsac), null },
        };

        public static void AddPreloadOverride(string propertyName, PreloadLevel value)
        {
            if (propertyName == null || !_preloadOverrides.TryGetValue(propertyName, out PreloadLevel? pl)) throw new ArgumentException("Invalid preload override name.");
            if (pl.HasValue && pl.Value != value)
            {
                LogWarn($"Incompatible overrides for preload setting {propertyName}. Replacing old override {pl} with new override {value}");
            }

            _preloadOverrides[propertyName] = value;
        }

        public static void RemovePreloadOverride(string propertyName)
        {
            if (propertyName == null || !_preloadOverrides.ContainsKey(propertyName)) throw new ArgumentException("Invalid preload override name.");
            _preloadOverrides[propertyName] = null;
        }

        internal void AddEntries(List<IMenuMod.MenuEntry> entries)
        {
            PreloadLevel[] values = Enum.GetValues(typeof(PreloadLevel)).Cast<PreloadLevel>().ToArray();
            string[] names = values.Select(p => p.ToString().FromCamelCase()).ToArray();
            foreach (string key in PreloadLevels.Keys)
            {
                entries.Add(new IMenuMod.MenuEntry(key.FromCamelCase(), names, string.Empty, 
                    i => PreloadLevels[key] = values[i], 
                    () => Array.IndexOf(values, PreloadLevels[key])));
            }
        }
    }
}
