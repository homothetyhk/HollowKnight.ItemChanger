using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public struct ObjectLocation : IMutableLocation
    {
        public string objectName;
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
            GameObject target = FindGameObject();

            switch (containerType)
            {
                case Container.GrubJar:
                    SetContext(obj, target);
                    GrubJarUtility.AdjustGrubJarPosition(obj, elevation);
                    break;
                case Container.GeoRock:
                    GeoRockUtility.SetRockContext(obj, target, elevation);
                    break;
                case Container.Chest:
                    ChestUtility.MoveChest(obj, target, elevation);
                    break;
                case Container.Shiny:
                default:
                    SetContext(obj, target);
                    break;
            }

            GameObject.Destroy(target);
        }

        public void SetContext(GameObject obj, GameObject target)
        {
            if (target.transform.parent != null)
            {
                obj.transform.SetParent(target.transform.parent);
            }

            obj.transform.position = target.transform.position;
            obj.transform.localPosition = target.transform.localPosition;
            obj.SetActive(target.activeSelf);
        }

        public GameObject FindGameObject()
        {
            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            string[] objectHierarchy = objectName.Split('\\');
            int i = 1;
            GameObject obj = currentScene.FindGameObject(objectHierarchy[0]);
            while (i < objectHierarchy.Length)
            {
                obj = obj.FindGameObjectInChildren(objectHierarchy[i++]);
            }

            return obj;
        }
    }

}
