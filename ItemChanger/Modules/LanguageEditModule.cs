using Newtonsoft.Json;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module for packaging language edits with an ItemChanger save.
    /// </summary>
    [DefaultModule]
    public class LanguageEditModule : Module
    {
        [JsonConverter(typeof(LanguageEditDictConverter))] public Dictionary<LanguageKey, LanguageEdit> languageEdits = new();

        public override void Initialize()
        {
            foreach (LanguageEdit e in languageEdits.Values) e.Hook();
        }

        public override void Unload()
        {
            foreach (LanguageEdit e in languageEdits.Values) e.Unhook();
        }

        public void AddLanguageEdit(LanguageKey key, string value, bool ignoreExistingOverrides = false)
        {
            if (languageEdits.TryGetValue(key, out LanguageEdit oldValue))
            {
                oldValue.Unhook();
            }
            languageEdits[key] = new()
            {
                Sheet = key.Sheet,
                Key = key.Key,
                Value = value,
                IgnoreExistingOverrides = ignoreExistingOverrides,
            };
        }

        public void RemoveLanguageEdit(LanguageKey key)
        {
            if (languageEdits.TryGetValue(key, out LanguageEdit oldValue))
            {
                oldValue.Unhook();
            }
            languageEdits.Remove(key);
        }

        public record LanguageEdit
        {
            public string? Sheet { get; init; }
            public string Key { get; init; }
            public string Value { get; init; }
            /// <summary>
            /// If false, and the key has been set to a nonempty value in a language.xml included with ItemChanger, then the language edit will not run.
            /// </summary>
            public bool IgnoreExistingOverrides { get; init; }

            [JsonIgnore] public LanguageKey LanguageKey { get => Sheet is null ? new(Key) : new(Sheet, Key); }

            public void Hook()
            {
                Events.AddLanguageEdit(LanguageKey, Edit);
            }

            public void Unhook()
            {
                Events.AddLanguageEdit(LanguageKey, Edit);
            }

            public void Edit(ref string value)
            {
                if (!IgnoreExistingOverrides)
                {
                    if (!string.IsNullOrEmpty(Internal.LanguageStringManager.GetICString(Key, Sheet!))) return;
                }

                value = Value;
            }
        }

        public class LanguageEditDictConverter : JsonConverter<Dictionary<LanguageKey, LanguageEdit>>
        {
            public override Dictionary<LanguageKey, LanguageEdit>? ReadJson(JsonReader reader, Type objectType, Dictionary<LanguageKey, LanguageEdit>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize<List<LanguageEdit>>(reader).ToDictionary(e => e.LanguageKey);
            }

            public override void WriteJson(JsonWriter writer, Dictionary<LanguageKey, LanguageEdit>? value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value?.Values?.ToList());
            }
        }
    }
}
