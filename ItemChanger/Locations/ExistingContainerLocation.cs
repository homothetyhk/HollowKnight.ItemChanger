namespace ItemChanger.Locations
{
    /// <summary>
    /// An abstract location which can be optionally replaced with a container, but has a natural way of giving items if not replaced.
    /// </summary>
    public abstract class ExistingContainerLocation : AbstractLocation
    {
        public string containerType;
        public bool nonreplaceable;

        public virtual bool WillBeReplaced()
        {
            return !(Placement.MainContainerType == containerType || nonreplaceable || Container.GetContainer(Placement.MainContainerType) is not Container c || !c.SupportsInstantiate);
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.ExistingContainerPlacement(name)
            {
                Location = this,
            };
        }
    }
}
