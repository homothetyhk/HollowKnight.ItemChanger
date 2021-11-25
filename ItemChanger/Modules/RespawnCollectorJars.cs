using ItemChanger.Util;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which makes the glass jars in the Collector's room respawn after breaking.
    /// </summary>
    [DefaultModule]
    public class RespawnCollectorJars : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Ruins2_11, RespawnJars);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Ruins2_11, RespawnJars);
        }

        private void RespawnJars(Scene scene)
        {
            SceneDataUtil.Save("Ruins2_11", "Break Jar", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (1)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (2)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (3)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (4)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (5)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (6)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (7)", false);
            SceneDataUtil.Save("Ruins2_11", "Break Jar (8)", false);
        }
    }
}
