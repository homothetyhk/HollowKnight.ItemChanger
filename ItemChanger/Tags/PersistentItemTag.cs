namespace ItemChanger.Tags
{
    public class PersistentItemTag : Tag, IPersistenceTag
    {
        public Persistence Persistence { get; set; }
    }
}
