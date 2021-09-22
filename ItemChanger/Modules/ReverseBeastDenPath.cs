using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
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
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Deepnest_Spider_Town",
                id = "Collapser Small (12)",
                activated = true,
                semiPersistent = false
            });
        }
    }
}
