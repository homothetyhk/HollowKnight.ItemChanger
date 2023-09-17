using ItemChanger.Extensions;
using ItemChanger.Locations;

namespace ItemChanger.Tags
{
    [TagConstrainedTo<ExistingContainerLocation>]
    public class DestroyOnECLReplaceTag : Tag
    {
        [Newtonsoft.Json.JsonIgnore]
        private ExistingContainerLocation location;
        public string objectPath;
        public string sceneName;

        public override void Load(object parent)
        {
            base.Load(parent);
            location = (ExistingContainerLocation)parent;
            Events.AddSceneChangeEdit(sceneName, OnSceneChange);
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);
            Events.RemoveSceneChangeEdit(sceneName, OnSceneChange);
        }

        private void OnSceneChange(Scene to)
        {
            if (location.WillBeReplaced())
            {
                UObject.Destroy(to.FindGameObject(objectPath));
            }
        }
    }
}
