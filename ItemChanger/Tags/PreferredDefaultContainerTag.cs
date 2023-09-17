namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for location or placement which indicates a preferred container type to use if no items request a specific container.
    /// <br/>Note that the default container without a tag is usually Shiny for one item or Chest for multiple items.
    /// </summary>
    [LocationTag]
    [PlacementTag]
    public class PreferredDefaultContainerTag : Tag
    {
        public string containerType;
    }
}
