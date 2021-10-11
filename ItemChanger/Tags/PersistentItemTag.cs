namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which indicates an item has a fixed persistence.
    /// </summary>
    public class PersistentItemTag : Tag, IPersistenceTag
    {
        public Persistence Persistence { get; set; }
    }
}
