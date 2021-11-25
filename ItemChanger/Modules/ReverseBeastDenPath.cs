using ItemChanger.Util;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which allows the deepest part of Beast's Den to be reached from the secret shortcut, by destroying the breakable floor above the grub.
    /// </summary>
    [DefaultModule]
    public class ReverseBeastDenPath : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Deepnest_Spider_Town, SaveBeastsDenCollapserOpen);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Deepnest_Spider_Town, SaveBeastsDenCollapserOpen);
        }

        private void SaveBeastsDenCollapserOpen(Scene scene)
        {
            SceneDataUtil.Save("Deepnest_Spider_Town", "Collapser Small (12)");
        }
    }
}
