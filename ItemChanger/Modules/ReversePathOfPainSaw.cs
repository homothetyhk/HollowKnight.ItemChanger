using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which adds saws to the first room of Path of Pain to allow it to be traversed in reverse.
    /// </summary>
    public class ReversePathOfPainSaw : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.White_Palace_18, MakeExtraSaws);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.White_Palace_18, MakeExtraSaws);
        }

        private void MakeExtraSaws(Scene scene)
        {
            const float SAW = 1.362954f;
            GameObject saw = scene.FindGameObject("saw_collection/wp_saw (4)");

            GameObject topSaw = UObject.Instantiate(saw);
            topSaw.transform.SetPositionX(165f);
            topSaw.transform.SetPositionY(30.5f);
            topSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

            GameObject botSaw = UObject.Instantiate(saw);
            botSaw.transform.SetPositionX(161.4f);
            botSaw.transform.SetPositionY(21.4f);
            botSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);
        }
    }
}
