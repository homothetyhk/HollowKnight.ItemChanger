using System.Runtime.CompilerServices;
using ItemChanger.Extensions;
using Modding;
using Newtonsoft.Json;
using MenuEntry = Modding.IMenuMod.MenuEntry;

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

        public PreloadSettings()
        {
            PreloadLevels = new()
            {
                { nameof(PreloadGeoRocks), PreloadLevel.Full },
                { nameof(PreloadSoulTotems), PreloadLevel.Full },
                { nameof(PreloadGrub), PreloadLevel.Full },
                { nameof(PreloadMimic), PreloadLevel.Full },
            };
        }
        [JsonConstructor] public PreloadSettings(Dictionary<string, PreloadLevel> PreloadLevels) => this.PreloadLevels = PreloadLevels;

        private PreloadLevel Get([CallerMemberName] string? caller = null)
        {
            if (_preloadOverrides[caller!] is PreloadLevel pl) return pl;
            else return PreloadLevels[caller!];
        }

        private void Set(PreloadLevel value, [CallerMemberName] string? caller = null) => PreloadLevels[caller!] = value;


        private static readonly Dictionary<string, PreloadLevel?> _preloadOverrides = new()
        {
            { nameof(PreloadGeoRocks), null },
            { nameof(PreloadSoulTotems), null },
            { nameof(PreloadGrub), null },
            { nameof(PreloadMimic), null },
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

        internal MenuEntry[] GetMenuEntries()
        {
            PreloadLevel[] values = Enum.GetValues(typeof(PreloadLevel)).Cast<PreloadLevel>().ToArray();
            string[] names = values.Select(p => LanguageStringManager.GetICString("PRELOAD_LEVEL_" + p.ToString().FromCamelCase().Replace(' ', '_').ToUpper() + "_NAME")).ToArray();
            return PreloadLevels.Keys.Select(key => new MenuEntry(
                    name: LanguageStringManager.GetICString(key.FromCamelCase().Replace(' ', '_').ToUpper() + "_NAME"),
                    values: names,
                    description: string.Empty,
                    saver: i => PreloadLevels[key] = values[i],
                    loader: () => Array.IndexOf(values, PreloadLevels[key])))
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .ToArray();
        }
    }
}
