using ItemChanger.Locations;

namespace ItemChanger.Placements
{
    /// <summary>
    /// Placement for self-implementing locations, usually acting through cutscene or conversation fsms.
    /// </summary>
    public class AutoPlacement : AbstractPlacement, IPrimaryLocationPlacement, ISingleCostPlacement
    {
        public AutoPlacement(string Name) : base(Name) { }

        public AutoLocation Location;

        AbstractLocation IPrimaryLocationPlacement.Location => Location;

        public Cost? Cost { get; set; }
        public virtual bool SupportsCost => Location.SupportsCost;

        protected override void OnLoad()
        {
            Location.Placement = this;
            Location.Load();
        }

        protected override void OnUnload()
        {
            Location.Unload();
        }

        public override IEnumerable<Tag> GetPlacementAndLocationTags()
        {
            return base.GetPlacementAndLocationTags().Concat(Location.tags ?? Enumerable.Empty<Tag>());
        }
    }
}
