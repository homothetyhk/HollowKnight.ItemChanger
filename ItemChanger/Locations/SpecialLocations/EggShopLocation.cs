using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Bad location to allow auto-wrapping a PlaceableLocation as an EggShopPlacement (instead of MutablePlacement)
    /// </summary>
    public class EggShopLocation : AbstractLocation
    {
        public PlaceableLocation location;

        public override AbstractPlacement Wrap()
        {
            return new Placements.EggShopPlacement(name)
            {
                Location = location,
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
