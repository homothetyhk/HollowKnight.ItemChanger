namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which adds a platform to Right City, replacing a Great Husk Sentry pogo that was removed when its patrol range was changed in the Lifeblood update.
    /// </summary>
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
