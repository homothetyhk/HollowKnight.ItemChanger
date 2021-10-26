using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Locations;
using Newtonsoft.Json;

namespace ItemChanger.Placements
{
    /// <summary>
    /// Placement which supports modifying existing containers in place or replacing them with a container preferred by the item list.
    /// </summary>
    public class ExistingContainerPlacement : AbstractPlacement, ISingleCostPlacement, IPrimaryLocationPlacement
    {
        public ExistingContainerPlacement(string Name) : base(Name) { }

        public ExistingContainerLocation Location;
        AbstractLocation IPrimaryLocationPlacement.Location => Location;

        [JsonProperty]
        private string currentContainerType = Container.Unknown;
        [JsonIgnore]
        public override string MainContainerType => currentContainerType;

        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            Location.Placement = this;
            UpdateContainerType();
            Location.Load();
        }

        protected override void OnUnload()
        {
            Location.Unload();
        }

        private void UpdateContainerType()
        {
            if (currentContainerType == Location.containerType) return;
            if (Location.nonreplaceable)
            {
                currentContainerType = Location.containerType;
                return;
            }
            if (Container.GetContainer(currentContainerType) is Container c && c.SupportsInstantiate) return;

            currentContainerType = ChooseContainerType(this, Location, Items);
        }

        public static string ChooseContainerType(ISingleCostPlacement placement, ExistingContainerLocation location, IEnumerable<AbstractItem> items)
        {
            if (location.nonreplaceable) return location.containerType;

            bool mustSupportCost = placement.Cost != null;
            bool mustSupportSceneChange = location.GetTags<Tags.ChangeSceneTag>().Any() || (placement as AbstractPlacement).GetTags<Tags.ChangeSceneTag>().Any();

            string containerType = items
                .Select(i => i.GetPreferredContainer())
                .FirstOrDefault(c => Container.GetContainer(c) is Container container && container.SupportsInstantiate && 
                (!mustSupportCost || container.SupportsCost) && 
                (!mustSupportSceneChange || container.SupportsSceneChange));

            if (string.IsNullOrEmpty(containerType))
            {
                return mustSupportCost ? Container.Shiny : location.containerType;
            }

            return containerType;
        }

        public override IEnumerable<Tag> GetPlacementAndLocationTags()
        {
            return base.GetPlacementAndLocationTags().Concat(Location.tags ?? Enumerable.Empty<Tag>());
        }
    }
}
