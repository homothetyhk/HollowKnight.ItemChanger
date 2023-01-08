using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which failed to deserialize. Contains the raw data of the module and the error which prevented deserialization.
    /// </summary>
    [JsonConverter(typeof(InvalidModuleConverter))]
    public sealed class InvalidModule : Module
    {
        /// <summary>
        /// The raw data of the module, as a JToken.
        /// </summary>
        public JToken JSON { get; init; }
        /// <summary>
        /// The error thrown during deserialization.
        /// </summary>
        public Exception DeserializationError { get; init; }

        public override void Initialize() { }
        public override void Unload() { }

        /// <summary>
        /// Converter which erases the InvalidTag during serialization and writes the JSON which it wraps.
        /// </summary>
        public class InvalidModuleConverter : JsonConverter<InvalidModule>
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;

            public override InvalidModule ReadJson(JsonReader reader, Type objectType, InvalidModule existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, InvalidModule value, JsonSerializer serializer)
            {
                value.JSON.WriteTo(writer);
            }
        }
    }
}
