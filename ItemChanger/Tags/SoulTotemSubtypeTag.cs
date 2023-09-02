namespace ItemChanger.Tags
{
    /// <summary>
    /// A <see cref="PreferredDefaultContainerTag"/> for <see cref="Container.Totem"/>, which allows specifying the <see cref="SoulTotemSubtype"/>.
    /// </summary>
    public class SoulTotemSubtypeTag : PreferredDefaultContainerTag
    {
        public SoulTotemSubtype Type;
        public SoulTotemSubtypeTag()
        {
            base.containerType = Container.Totem;
        }
    }
}
