using ItemChanger.Tags;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ItemChanger
{
    public class TaggableObject
    {
        [JsonProperty] [JsonConverter(typeof(TagListDeserializer))] public List<Tag>? tags;
        private bool _tagsLoaded;

        protected void LoadTags()
        {
            _tagsLoaded = true;
            if (tags == null) return;
            for (int i = 0; i < tags.Count; i++)
            {
                tags[i].LoadOnce(this);
            }
        }

        protected void UnloadTags()
        {
            _tagsLoaded = false;
            if (tags == null) return;
            for (int i = 0; i < tags.Count; i++)
            {
                tags[i].UnloadOnce(this);
            }
        }

        public T AddTag<T>() where T : Tag, new()
        {
            tags ??= new();
            T t = new();
            if (_tagsLoaded) t.LoadOnce(this);
            tags.Add(t);
            return t;
        }

        public void AddTag(Tag t)
        {
            tags ??= new();
            if (_tagsLoaded) t.LoadOnce(this);
            tags.Add(t);
        }

        public void AddTags(IEnumerable<Tag> ts)
        {
            tags ??= new();
            if (_tagsLoaded) foreach (Tag t in ts) t.LoadOnce(this);
            tags.AddRange(ts);
        }

        public T? GetTag<T>()
        {
            return tags == null ? default : tags.OfType<T>().FirstOrDefault();
        }

        public bool GetTag<T>(out T t) where T : class
        {
            t = GetTag<T>()!;
            return t != null;
        }

        public IEnumerable<T> GetTags<T>()
        {
            return tags?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        public T GetOrAddTag<T>() where T : Tag, new()
        {
            tags ??= new List<Tag>();
            return tags.OfType<T>().FirstOrDefault() ?? AddTag<T>();
        }

        public bool HasTag<T>() where T : Tag
        {
            return tags?.OfType<T>()?.Any() ?? false;
        }

        public void RemoveTags<T>()
        {
            if (_tagsLoaded && tags != null)
            {
                foreach (Tag t in tags.Where(t => t is T)) t.UnloadOnce(this);
            }
            tags = tags?.Where(t => t is not T)?.ToList();
        }
        
        public class TagListSerializer : JsonConverter<List<Tag>>
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;
            public bool RemoveNewProfileTags;
            public override List<Tag>? ReadJson(JsonReader reader, Type objectType, List<Tag>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
            public override void WriteJson(JsonWriter writer, List<Tag>? value, JsonSerializer serializer)
            {
                if (value is null)
                {
                    writer.WriteNull();
                    return;
                }

                if (RemoveNewProfileTags)
                {
                    value = new(value.Where(t => !t.TagHandlingProperties.HasFlag(TagHandlingFlags.RemoveOnNewProfile)));
                }

                serializer.Serialize(writer, value.ToArray());
            }
        }

        public class TagListDeserializer : JsonConverter<List<Tag>>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override List<Tag>? ReadJson(JsonReader reader, Type objectType, List<Tag>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JToken jt = JToken.Load(reader);
                if (jt.Type == JTokenType.Null) return null;
                else if (jt.Type == JTokenType.Array)
                {
                    JArray ja = (JArray)jt;
                    List<Tag> list = new(ja.Count);
                    foreach (JToken jTag in ja)
                    {
                        Tag t;
                        try
                        {
                            t = jTag.ToObject<Tag>(serializer)!;
                        }
                        catch (Exception e)
                        {
                            TagHandlingFlags flags = ((JObject)jTag).GetValue(nameof(Tag.TagHandlingProperties))?.ToObject<TagHandlingFlags>(serializer) ?? TagHandlingFlags.None;
                            if (flags.HasFlag(TagHandlingFlags.AllowDeserializationFailure))
                            {
                                t = new InvalidTag
                                {
                                    JSON = jTag,
                                    DeserializationError = e,
                                };
                            }
                            else throw;
                        }
                        list.Add(t);
                    }
                    return list;
                }
                throw new JsonSerializationException("Unable to handle tag list pattern: " + jt.ToString());
            }

            public override void WriteJson(JsonWriter writer, List<Tag>? value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
