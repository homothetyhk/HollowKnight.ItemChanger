using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;

namespace ItemChanger.Placements
{
    /// <summary>
    /// A simple placement which attempts to give all of its remaining items on entering any active scene.
    /// </summary>
    public class StartPlacement : AbstractPlacement
    {
        public StartLocation location;
        public override AbstractLocation Location => location;
    }
}
