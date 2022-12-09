using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which failed to deserialize. Contains the raw data of the tag and the error which prevented deserialization.
    /// </summary>
    [JsonConverter(typeof(InvalidTagConverter))]
    public sealed class InvalidTag : Tag
    {
        /// <summary>
        /// The raw data of the tag, as a JToken.
        /// </summary>
        public JToken JSON { get; init; }
        /// <summary>
        /// The error thrown during deserialization.
        /// </summary>
        public Exception DeserializationError { get; init; }

        /// <summary>
        /// Converter which erases the InvalidTag during serialization and writes the JSON which it wraps.
        /// </summary>
        public class InvalidTagConverter : JsonConverter<InvalidTag>
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;

            public override InvalidTag ReadJson(JsonReader reader, Type objectType, InvalidTag existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, InvalidTag value, JsonSerializer serializer)
            {
                value.JSON.WriteTo(writer);
            }
        }
    }
}
