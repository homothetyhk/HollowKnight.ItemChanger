using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Custom
{
    public static class Locations
    {
        public static _Location CreateCustomLocation(string name, string sceneName, float x, float y, int cost = 0, CostType costType = CostType.Geo)
        {
            return new _Location
            {
                name = name,
                sceneName = sceneName,
                x = x,
                y = y,
                newShiny = true,
                cost = cost,
                costType = costType,
            };
        }

        public static _Location EditCost(_Location loc, int cost, CostType costType)
        {
            loc.cost = cost;
            loc.costType = costType;
            return loc;
        }

        public static _Location EditCost(_Location loc, int cost, string costType)
        {
            loc.cost = cost;
            loc.costType = (CostType)Enum.Parse(typeof(CostType), costType);
            return loc;
        }
    }
}
