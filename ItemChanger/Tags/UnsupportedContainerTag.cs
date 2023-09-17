namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for a location or placement to indicate that a container is not supported and should not be chosen.
    /// </summary>
    [LocationTag]
    [PlacementTag]
    public class UnsupportedContainerTag : Tag
    {
        public string containerType;
    }
}
