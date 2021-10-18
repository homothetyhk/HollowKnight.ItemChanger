using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class RightCityPlatform : Module
    {
        private SmallPlatform _plat;

        public override void Initialize()
        {
            _plat ??= new()
            {
                SceneName = SceneNames.Ruins2_04,
                X = 18f,
                Y = 10f,
            };

            Events.AddSceneChangeEdit(SceneNames.Ruins2_04, _plat.OnSceneChange);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Ruins2_04, _plat.OnSceneChange);
        }
    }
}
