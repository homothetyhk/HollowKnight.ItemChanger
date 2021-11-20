using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class GreatHopperEasterEgg : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Deepnest_East_16, EditScene);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Deepnest_East_16, EditScene);
        }

        private void EditScene(Scene newScene)
        {
            GameObject hopper1 = newScene.FindGameObject("Giant Hopper Summon/Giant Hopper");
            GameObject hopper2 = newScene.FindGameObject("Giant Hopper Summon/Giant Hopper (1)");

            for (int i = 0; i < 10; i++)
            {
                GameObject newHopper1 = UObject.Instantiate(hopper1, hopper1.transform.parent);
                GameObject newHopper2 = UObject.Instantiate(hopper2, hopper2.transform.parent);

                HealthManager hopper1HM = newHopper1.GetComponent<HealthManager>();
                hopper1HM.SetGeoSmall(0);
                hopper1HM.SetGeoMedium(0);
                hopper1HM.SetGeoLarge(0);

                HealthManager hopper2HM = newHopper2.GetComponent<HealthManager>();
                hopper2HM.SetGeoSmall(0);
                hopper2HM.SetGeoMedium(0);
                hopper2HM.SetGeoLarge(0);

                Vector3 hopper1Pos = newHopper1.transform.localPosition;
                hopper1Pos = new Vector3(
                    hopper1Pos.x + i,
                    hopper1Pos.y,
                    hopper1Pos.z);
                newHopper1.transform.localPosition = hopper1Pos;

                Vector3 hopper2Pos = newHopper2.transform.localPosition;
                hopper2Pos = new Vector3(
                    hopper2Pos.x + i - 4,
                    hopper2Pos.y,
                    hopper2Pos.z);
                newHopper2.transform.localPosition = hopper2Pos;
            }
        }
    }
}
