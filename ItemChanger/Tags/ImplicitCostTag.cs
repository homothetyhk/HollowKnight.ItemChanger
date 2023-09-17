namespace ItemChanger.Tags
{
    /// <summary>
    /// A tag which does not modify behavior, but provides information about the implicit costs of a placement or location.
    /// </summary>
    [LocationTag]
    [PlacementTag]
    public class ImplicitCostTag : Tag
    {
        public Cost Cost;
        /// <summary>
        /// An inherent cost always applies. A non-inherent cost applies as a substitute when the placement does not have a (non-null) cost.
        /// </summary>
        public bool Inherent;

        public override void Load(object parent)
        {
            base.Load(parent);
            Cost.Load();
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);
            Cost.Unload();
        }
    }
}
