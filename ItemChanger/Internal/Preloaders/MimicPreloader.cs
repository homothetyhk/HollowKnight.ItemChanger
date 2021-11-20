namespace ItemChanger.Internal.Preloaders
{
    public class MimicPreloader : Preloader
    {
        public override IEnumerable<(string, string)> GetPreloadNames()
        {
            if (PreloadLevel != PreloadLevel.None)
            {
                yield return (SceneNames.Deepnest_36, "Grub Mimic Bottle");
                yield return (SceneNames.Deepnest_36, "Grub Mimic Top");
                //(SceneNames.Deepnest_36, "Dream Dialogue"),
            }
        }

        public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            if (PreloadLevel != PreloadLevel.None)
            {
                _mimicBottle = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Bottle"];
                _mimicTop = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Top"];
            }
        }

        private GameObject _mimicBottle;
        private GameObject _mimicTop;
        //private GameObject _mimicDialogue;

        public GameObject MimicBottle
        {
            get
            {
                if (PreloadLevel == PreloadLevel.None) throw NotPreloadedException();
                return UObject.Instantiate(_mimicBottle);
            }
        }

        public GameObject MimicTop
        {
            get
            {
                if (PreloadLevel == PreloadLevel.None) throw NotPreloadedException();
                return UObject.Instantiate(_mimicTop);
            }
        }
    }
}
