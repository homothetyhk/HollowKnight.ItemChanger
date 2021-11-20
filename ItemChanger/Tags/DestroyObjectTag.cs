namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for destroying an object in a specific scene. Can search by name or by path.
    /// </summary>
    public class DestroyObjectTag : Tag
    {
        public string objectName;
        public string sceneName;

        public override void Load(object parent)
        {
            Events.AddSceneChangeEdit(sceneName, DestroyObject);
        }

        public override void Unload(object parent)
        {
            Events.RemoveSceneChangeEdit(sceneName, DestroyObject);
        }

        public void DestroyObject(Scene to)
        {
            if (to.name == sceneName)
            {
                GameObject obj = Locations.ObjectLocation.FindGameObject(objectName);
                if (obj)
                {
                    GameObject.Destroy(obj);
                    //Log($"Destroyed object {objectName} in {sceneName}");
                }
                //else Log($"Could not find object {objectName} in {sceneName}");
            }
        }
    }
}
