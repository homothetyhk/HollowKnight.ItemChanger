using ItemChanger.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class CoordinateLocation : PlaceableLocation
    {
        public float x;
        public float y;
        public float elevation;

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (!auxillary)
            {
                base.GetPrimaryContainer(out GameObject obj, out Container containerType);
                PlaceContainer(obj, containerType);
            }
        }

        public override void PlaceContainer(GameObject obj, Container containerType)
        {
            switch (containerType)
            {
                case Container.GrubJar:
                    GrubJarUtility.MoveGrubJar(obj, x, y, elevation);
                    break;
                case Container.GeoRock:
                    GeoRockUtility.SetRockContext(obj, x, y, elevation);
                    break;
                case Container.Chest:
                    ChestUtility.MoveChest(obj, x, y, elevation);
                    break;
                case Container.Tablet:
                    obj.transform.position = new Vector3(x, y - elevation, 2.5f);
                    obj.SetActive(true);
                    break;
                case Container.Shiny:
                default:
                    obj.transform.position = new Vector3(x, y, 0);
                    obj.SetActive(true);
                    break;
            }
        }
    }
}
