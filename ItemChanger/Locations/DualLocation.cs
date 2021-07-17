using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Helper location meant to be unpacked into a DualPlacement
    /// </summary>
    public class DualLocation : AbstractLocation
    {
        public IBool Test;
        public AbstractLocation falseLocation;
        public AbstractLocation trueLocation;

        public override AbstractPlacement Wrap()
        {
            return new Placements.DualPlacement
            {
                Test = Test,
                falseLocation = falseLocation,
                trueLocation = trueLocation,   
            };
        }
    }
}
