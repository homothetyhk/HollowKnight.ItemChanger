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
            GameObject cliffsCrawlid = UObject.Instantiate(GameObject.Find("Crawler"));
            cliffsCrawlid.SetActive(true);
            cliffsCrawlid.transform.position = new Vector2(74f, 31f);
            foreach (GameObject g in UObject.FindObjectsOfType<GameObject>())
            {
                if (g.transform.GetPositionX() < 75 && g.transform.GetPositionX() > 70 && g.transform.GetPositionY() < 54 && g.transform.GetPositionY() > 33)
                {
                    UObject.Destroy(g);
                }
            }
        }

    }
}
