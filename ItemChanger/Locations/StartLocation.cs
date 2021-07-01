using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    public class StartLocation : AbstractLocation
    {
        public override void OnLoad()
        {
            base.OnLoad();
            Placement.GiveAll(MessageType.Corner);
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.StartPlacement
            {
                location = this,
            };
        }
    }
}
