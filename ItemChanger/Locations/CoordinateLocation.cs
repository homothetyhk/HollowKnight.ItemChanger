using ItemChanger.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Location which places a container at a specified coordinate position.
    /// </summary>
    public class CoordinateLocation : PlaceableLocation
    {
        public float x;
        public float y;
        public float elevation;

        protected override void OnLoad()
        {
            Events.AddSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        protected override void OnUnload()
        {
            Events.RemoveSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        public void OnActiveSceneChanged(Scene to)
        {
            if (!managed && to.name == sceneName)
            {
                base.GetContainer(out GameObject obj, out string containerType);
                PlaceContainer(obj, containerType);
            }
        }

        public override void PlaceContainer(GameObject obj, string containerType)
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
