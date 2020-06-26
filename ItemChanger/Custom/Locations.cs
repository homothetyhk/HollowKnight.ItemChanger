using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Custom
{
    public static class Locations
    {
        public static Location CreateCustomLocation(string name, string sceneName, float x, float y, int cost = 0, Location.CostType costType = Location.CostType.Geo)
        {
            return new Location
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

        public static Location EditCost(Location loc, int cost, Location.CostType costType)
        {
            loc.cost = cost;
            loc.costType = costType;
            return loc;
        }

        public static Location EditCost(Location loc, int cost, string costType)
        {
            loc.cost = cost;
            loc.costType = (Location.CostType)Enum.Parse(typeof(Location.CostType), costType);
            return loc;
        }
    }
}
