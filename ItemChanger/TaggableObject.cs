namespace ItemChanger
{
    public class TaggableObject
    {
        [Newtonsoft.Json.JsonProperty]
        public List<Tag> tags;

        protected void LoadTags()
        {
            if (tags == null) return;
            foreach (Tag tag in tags) tag.Load(this);
        }

        protected void UnloadTags()
        {
            if (tags == null) return;
            foreach (Tag tag in tags) tag.Unload(this);
        }

        public T AddTag<T>() where T : Tag, new()
        {
            if (tags == null) tags = new List<Tag>();
            T t = new T();
            tags.Add(t);
            return t;
        }

        public void AddTags(IEnumerable<Tag> ts)
        {
            if (tags == null) tags = new List<Tag>();
            tags.AddRange(ts);
        }

        public T GetTag<T>()
        {
            if (tags == null) return default;
            return tags.OfType<T>().FirstOrDefault();
        }

        public bool GetTag<T>(out T t) where T : class
        {
            t = GetTag<T>();
            return t != null;
        }

        public IEnumerable<T> GetTags<T>()
        {
            return tags?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        public T GetOrAddTag<T>() where T : Tag, new()
        {
            if (tags == null) tags = new List<Tag>();
            return tags.OfType<T>().FirstOrDefault() ?? AddTag<T>();
        }

        public bool HasTag<T>() where T : Tag
        {
            return tags?.OfType<T>()?.Any() ?? false;
        }

        public void RemoveTags<T>()
        {
            tags = tags?.Where(t => !(t is T))?.ToList();
        }
    }
}
