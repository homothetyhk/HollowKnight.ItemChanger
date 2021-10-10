using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ItemChanger.Internal.Preloaders
{
    public class GrubPreloader : Preloader
    {
        public override IEnumerable<(string, string)> GetPreloadNames()
        {
            if (PreloadLevel != PreloadLevel.None) yield return (SceneNames.Deepnest_36, "Grub Bottle");
        }

        public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            if (PreloadLevel != PreloadLevel.None) _grubJar = objectsByScene[SceneNames.Deepnest_36]["Grub Bottle"];
        }


        private GameObject _grubJar;
        public GameObject GrubJar
        {
            get
            {
                if (PreloadLevel == PreloadLevel.None) throw NotPreloadedException();
                return UObject.Instantiate(_grubJar);
            }
        }
    }
}
