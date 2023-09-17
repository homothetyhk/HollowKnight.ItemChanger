namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which indicates an item has a fixed persistence.
    /// </summary>
    [ItemTag]
    public class PersistentItemTag : Tag, IPersistenceTag
    {
        public Persistence Persistence { get; set; }
    }
}
