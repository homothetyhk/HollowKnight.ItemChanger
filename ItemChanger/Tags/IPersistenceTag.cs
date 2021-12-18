namespace ItemChanger.Tags
{
    /// <summary>
    /// Interface used when ItemChanger checks tags to determine whether an item is persistent or semipersistent, and should be refreshed.
    /// </summary>
    public interface IPersistenceTag
    {
        Persistence Persistence { get; }
    }
}
