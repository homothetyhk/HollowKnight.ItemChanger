using ItemChanger.Placements;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Wrapper location to allow converting a ContainerLocation, PlaceableLocation pair to a CostChestPlacement.
    /// </summary>
    public class CostChestLocation : AbstractLocation
    {
        public ContainerLocation chestLocation;
        public PlaceableLocation tabletLocation;

        public override AbstractPlacement Wrap()
        {
            return new CostChestPlacement(name)
            {
                chestLocation = chestLocation,
                tabletLocation = tabletLocation,
                tags = tags,
            };
        }

        protected override void OnLoad()
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}
