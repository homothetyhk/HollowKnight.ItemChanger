namespace ItemChanger.Locations
{
    /// <summary>
    /// A location with no effects. Use, for example, with DualPlacement, or in other situations where a dummy location may be needed.
    /// </summary>
    public class EmptyLocation : AutoLocation
    {
        protected override void OnLoad() { }
        protected override void OnUnload() { }
    }
}
