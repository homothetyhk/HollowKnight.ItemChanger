using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class ContainerUtility
    {
        public static GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, Container type)
        {
            switch (type)
            {
                default:
                case Container.Shiny:
                    return ShinyUtility.MakeNewMultiItemShiny(placement);
                case Container.Chest:
                    return ChestUtility.MakeNewChest(placement);
                case Container.GeoRock:
                    return GeoRockUtility.MakeNewGeoRock(placement, items, out _);
                case Container.GrubJar:
                    return GrubJarUtility.MakeNewGrubJar(placement);
            }
        }
    }
}
