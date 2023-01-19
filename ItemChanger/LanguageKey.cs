using Newtonsoft.Json;

namespace ItemChanger
{
    public readonly struct LanguageKey
    {
        /// <summary>
        /// Create an LanguageKey which matches any call with this key.
        /// </summary>
        public LanguageKey(string Key)
        {
            this.Sheet = null;
            this.Key = Key;
        }

        /// <summary>
        /// Create an LanguageKey which matches any call with this sheet and key.
        /// </summary>
        public LanguageKey(string Sheet, string Key)
        {
            this.Sheet = Sheet;
            this.Key = Key;
        }

        public readonly string? Sheet;
        public readonly string Key;

        public override int GetHashCode()
        {
            int h1 = Sheet?.GetHashCode() ?? 0;
            int h2 = Key.GetHashCode();

            return ((h1 << 5) + h1) ^ h2;
        }

        public override string ToString()
        {
            return Sheet == null ? Key : $"{Sheet}-{Key}";
        }

        public class LanguageKeyDictConverter<TValue> : JsonConverter<Dictionary<LanguageKey, TValue>>
        {
            public override Dictionary<LanguageKey, TValue>? ReadJson(
                JsonReader reader,
                Type objectType,
                Dictionary<LanguageKey, TValue>? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                return serializer.Deserialize<KeyValuePair<LanguageKey, TValue>[]>(reader)?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public override void WriteJson(JsonWriter writer, Dictionary<LanguageKey, TValue>? value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value?.ToArray());
            }
        }
    }
}
