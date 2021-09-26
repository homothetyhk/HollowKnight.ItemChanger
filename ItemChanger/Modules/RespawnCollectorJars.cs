using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
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
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (1)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (2)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (3)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (4)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (5)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (6)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (7)",
                activated = false,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins2_11",
                id = "Break Jar (8)",
                activated = false,
                semiPersistent = false
            });
        }
    }
}
