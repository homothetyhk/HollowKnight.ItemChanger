using ItemChanger.Util;
using UnityEngine;

namespace ItemChanger.Locations
{
    public class CoordinateLocation : IMutableLocation
    {
        public float x;
        public float y;
        public float elevation;
        public string sceneName { get; set; }
        public FlingType flingType { get; set; }
        public bool forceShiny { get; set; }

        public bool Supports(Container container)
        {
            switch (container)
            {
                case Container.Chest:
                case Container.GeoRock:
                case Container.GrubJar:
                    return !forceShiny;
                case Container.Shiny:
                    return true;
                default:
                    return false;
            }
        }

        public void PlaceContainer(GameObject obj, Container containerType)
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

        public virtual void OnEnable(PlayMakerFSM fsm) { }
        public virtual void OnActiveSceneChanged() { }
        public virtual void Hook() { }
        public virtual void UnHook() { }
    }

}
