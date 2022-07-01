namespace ItemChanger.Placements
{
    /// <summary>
    /// Placement which handles switching between two possible locations according to a test.
    /// </summary>
    public class DualPlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement, IPrimaryLocationPlacement
    {
        public DualPlacement(string Name) : base(Name) { }

        public AbstractLocation trueLocation;
        public AbstractLocation falseLocation;

        public IBool Test;
        private bool cachedValue;

        public string containerType = Container.Unknown;
        public override string MainContainerType => containerType;

        [Newtonsoft.Json.JsonIgnore]
        public AbstractLocation Location => cachedValue ? trueLocation : falseLocation;
        
        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            cachedValue = Test.Value;
            trueLocation.Placement = this;
            falseLocation.Placement = this;
            SetContainerType();
            Location.Load();
            Cost?.Load();
            Events.OnBeginSceneTransition += OnBeginSceneTransition;
        }

        protected override void OnUnload()
        {
            Location.Unload();
            Cost?.Unload();
            Events.OnBeginSceneTransition -= OnBeginSceneTransition;
        }

        private void OnBeginSceneTransition(Transition obj)
        {
            bool value = Test.Value;
            if (cachedValue != value)
            {
                Location.Unload();
                cachedValue = value;
                Location.Load();
            }
        }

        // MutablePlacement implementation of GetContainer
        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = MutablePlacement.ChooseContainerType(this, location as Locations.ContainerLocation, Items);
            }

            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (containerType == null || !container.SupportsInstantiate)
            {
                // this means that the container that was chosen on load isn't valid
                // most likely due from switching from a noninstantiatable ECL to a CL
                // so, we make a shiny but we don't modify the saved container type
                containerType = Container.Shiny;
                container = Container.GetContainer(containerType);
            }

            obj = container.GetNewContainer(new ContainerInfo(container.Name, this, location.flingType, Cost, 
                location.GetTags<Tags.ChangeSceneTag>().FirstOrDefault()?.ToChangeSceneInfo()));
        }

        private void SetContainerType()
        {
            bool mustSupportCost = Cost != null;
            bool mustSupportSceneChange = falseLocation.GetTags<Tags.ChangeSceneTag>().Any() 
                || trueLocation.GetTags<Tags.ChangeSceneTag>().Any() || GetTags<Tags.ChangeSceneTag>().Any();
            if (Container.SupportsAll(containerType, true, mustSupportCost, mustSupportSceneChange)) return;

            if (falseLocation is Locations.ExistingContainerLocation fecl)
            {
                if (containerType == fecl.containerType && Container.SupportsAll(containerType, false, mustSupportCost, mustSupportSceneChange)) return;
                else
                {
                    containerType = ExistingContainerPlacement.ChooseContainerType(this, fecl, Items);
                    return;
                }
            }
            else if (trueLocation is Locations.ExistingContainerLocation tecl)
            {
                if (containerType == tecl.containerType && Container.SupportsAll(containerType, false, mustSupportCost, mustSupportSceneChange)) return;
                else
                {
                    containerType = ExistingContainerPlacement.ChooseContainerType(this, tecl, Items);
                    return;
                }
            }

            Locations.ContainerLocation cl = (falseLocation as Locations.ContainerLocation) ?? (trueLocation as Locations.ContainerLocation);
            if (cl == null) return;
            containerType = MutablePlacement.ChooseContainerType(this, cl, Items); // container type already failed the initial test
        }

        public override IEnumerable<Tag> GetPlacementAndLocationTags()
        {
            return base.GetPlacementAndLocationTags()
                .Concat(falseLocation.tags ?? Enumerable.Empty<Tag>())
                .Concat(trueLocation.tags ?? Enumerable.Empty<Tag>());
        }
    }
}
