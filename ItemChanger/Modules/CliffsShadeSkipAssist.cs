using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which removes scenery objects above the chest above Baldur shell, to make the passage to Howling Cliffs visible.
    /// </summary>
    [DefaultModule]
    public class CliffsShadeSkipAssist : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Fungus1_28, SpawnCrawlidAndUnhideShaft);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Fungus1_28, SpawnCrawlidAndUnhideShaft);
        }

        private void SpawnCrawlidAndUnhideShaft(Scene scene)
        {
            GameObject cliffsCrawlid = UObject.Instantiate(scene.FindGameObject("Crawler"));
            cliffsCrawlid.SetActive(true);
            cliffsCrawlid.transform.position = new Vector2(74f, 31f);

            foreach (GameObject g in scene.GetRootGameObjects())
            {
                if (objectNames.Contains(g.name)) UObject.Destroy(g);
            }
        }

        private static readonly HashSet<string> objectNames = new()
        {
            "cd_FG_rock_20 (3)",
            "roof_04 (5)",
            "Tut_msk_06",
            "cd_FG_rock_20 (1)",
            "cd_FG_rock_20",
            "cd_FG_rock_20 (199)",
            "cd_FG_rock_20 (231)",
            "cd_FG_rock_20 (230)",
            "cd_FG_rock_20 (7)",
        };
    }
}
