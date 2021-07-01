using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class ContainerUtility
    {
        public const string Shiny = "Shiny";
        public const string GrubJar = "GrubJar";
        public const string GeoRock = "GeoRock";
        public const string Chest = "Chest";

        public static GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, Container type)
        {
            switch (type)
            {
                default:
                case Container.Shiny:
                    return ShinyUtility.MakeNewMultiItemShiny(placement, items);
                case Container.Chest:
                    return ChestUtility.MakeNewChest(placement, items);
                case Container.GeoRock:
                    return GeoRockUtility.MakeNewGeoRock(placement, items, out _);
                case Container.GrubJar:
                    return GrubJarUtility.MakeNewGrubJar(placement, items);
            }
        }

        public static void ApplyTargetContext(GameObject target, GameObject container, Container containerType, float elevation)
        {
            switch (containerType)
            {
                case Container.GrubJar:
                    SetContext(target, container);
                    GrubJarUtility.AdjustGrubJarPosition(container, elevation);
                    break;
                case Container.GeoRock:
                    GeoRockUtility.SetRockContext(container, target, elevation);
                    break;
                case Container.Chest:
                    ChestUtility.MoveChest(container, target, elevation);
                    break;
                case Container.Shiny:
                default:
                    SetContext(target, container);
                    break;
            }
        }

        private static void SetContext(GameObject target, GameObject obj)
        {
            if (target.transform.parent != null)
            {
                obj.transform.SetParent(target.transform.parent);
            }

            obj.transform.position = target.transform.position;
            obj.transform.localPosition = target.transform.localPosition;
            obj.SetActive(target.activeSelf);
        }



    }
}
