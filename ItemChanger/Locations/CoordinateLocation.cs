using ItemChanger.Util;

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
            Container.GetContainer(containerType).ApplyTargetContext(obj, x, y, elevation);
            if (!obj.activeSelf) obj.SetActive(true);
        }
    }
}
