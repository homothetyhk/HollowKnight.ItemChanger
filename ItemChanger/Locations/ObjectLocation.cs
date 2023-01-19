using ItemChanger.Extensions;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Base type for finding and replacing a game object with an item container.
    /// </summary>
    public class ObjectLocation : ContainerLocation
    {
        public string objectName;
        public float elevation;

        protected override void OnLoad()
        {
            Events.AddSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        protected override void OnUnload()
        {
            Events.RemoveSceneChangeEdit(sceneName, OnActiveSceneChanged);
        }

        public virtual void OnActiveSceneChanged(Scene to)
        {
            base.GetContainer(out GameObject obj, out string containerType);
            PlaceContainer(obj, containerType);
        }

        public virtual void PlaceContainer(GameObject obj, string containerType)
        {
            GameObject target = FindGameObject(objectName);
            if (!target)
            {
                LogError($"Unable to find {objectName} for ObjectLocation {name}!");
                return;
            }

            Container.GetContainer(containerType)!.ApplyTargetContext(obj, target, elevation);
            UObject.Destroy(target);
        }


        public static GameObject FindGameObject(string objectName)
        {
            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            objectName = objectName.Replace('\\', '/');
            GameObject? obj;

            if (!objectName.Contains('/'))
            {
                obj = currentScene.FindGameObjectByName(objectName);
            }
            else
            {
                obj = currentScene.FindGameObject(objectName);
            }

            if (obj == null) LogWarn($"Failed to find {objectName} in scene {currentScene.name}!");

            return obj!;
        }
    }
}
