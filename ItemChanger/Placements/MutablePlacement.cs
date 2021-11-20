using ItemChanger.Locations;

namespace ItemChanger.Placements
{
    /// <summary>
    /// The default placement for most use cases.
    /// Chooses an item container for its location based on its item list.
    /// </summary>
    public class MutablePlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement, IPrimaryLocationPlacement
    {
        public MutablePlacement(string Name) : base(Name) { }

        public ContainerLocation Location;
        AbstractLocation IPrimaryLocationPlacement.Location => Location;

        public override string MainContainerType => containerType;
        public string containerType = Container.Unknown;

        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            Location.Placement = this;
            Location.Load();
        }

        protected override void OnUnload()
        {
            Location.Unload();
        }

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = ChooseContainerType(this, location as ContainerLocation, Items);
            }
            
            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (container == null || !container.SupportsInstantiate)
            {
                this.containerType = containerType = ChooseContainerType(this, location as ContainerLocation, Items);
                container = Container.GetContainer(containerType);
                if (container == null) throw new InvalidOperationException($"Unable to resolve container type {containerType} for placement {Name}!");
            }

            Transition? changeScene = location.GetTags<Tags.ChangeSceneTag>().Concat(GetTags<Tags.ChangeSceneTag>()).FirstOrDefault()?.changeTo;
            obj = container.GetNewContainer(this, Items, location.flingType, Cost, changeScene);
        }

        public static string ChooseContainerType(ISingleCostPlacement placement, ContainerLocation location, IEnumerable<AbstractItem> items)
        {
            if (location?.forceShiny ?? true)
            {
                return Container.Shiny;
            }

            bool mustSupportCost = placement.Cost != null;
            bool mustSupportSceneChange = location.GetTags<Tags.ChangeSceneTag>().Any() || (placement as AbstractPlacement).GetTags<Tags.ChangeSceneTag>().Any();

            HashSet<string> unsupported = new(((placement as AbstractPlacement)?.GetPlacementAndLocationTags() ?? Enumerable.Empty<Tag>())
                .OfType<Tags.UnsupportedContainerTag>()
                .Select(t => t.containerType));

            string containerType = items
                .Select(i => i.GetPreferredContainer())
                .FirstOrDefault(c => location.Supports(c) && !unsupported.Contains(c) && Container.SupportsAll(c, true, mustSupportCost, mustSupportSceneChange));

            if (string.IsNullOrEmpty(containerType))
            {
                if (((placement as AbstractPlacement)?.GetPlacementAndLocationTags() ?? Enumerable.Empty<Tag>())
                    .OfType<Tags.PreferredDefaultContainerTag>().FirstOrDefault() is Tags.PreferredDefaultContainerTag t
                    && Container.SupportsAll(t.containerType, true, mustSupportCost, mustSupportSceneChange))
                {
                    containerType = t.containerType;
                }
                else if (mustSupportCost || mustSupportSceneChange || items.Count() == 1) containerType = Container.Shiny;
                else containerType = Container.Chest;
            }

            return containerType;
        }

        public override IEnumerable<Tag> GetPlacementAndLocationTags()
        {
            return base.GetPlacementAndLocationTags().Concat(Location.tags ?? Enumerable.Empty<Tag>());
        }
    }
}
