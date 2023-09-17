namespace ItemChanger.Tags
{
    /// <summary>
    /// A <see cref="PreferredDefaultContainerTag"/> for <see cref="Container.GeoRock"/>, which allows specifying the <see cref="GeoRockSubtype"/>.
    /// </summary>
    [LocationTag]
    [PlacementTag]
    public class GeoRockSubtypeTag : PreferredDefaultContainerTag
    {
        public GeoRockSubtype Type;
        public GeoRockSubtypeTag()
        {
            base.containerType = Container.GeoRock;
        }
    }
}
