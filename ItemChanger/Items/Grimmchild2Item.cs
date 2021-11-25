using ItemChanger.Util;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives Grimmchild, activates the Nightmare Lantern, and auto-completes the first two flame collection quests.
    /// </summary>
    public class Grimmchild2Item : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.instance.gotCharm_40), true);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternAppeared), true);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternLit), true);
            PlayerData.instance.SetBool(nameof(PlayerData.troupeInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.divineInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.metGrimm), true);
            PlayerData.instance.SetInt(nameof(PlayerData.flamesRequired), 3);
            // Skip first two collection quests
            PlayerData.instance.IntAdd(nameof(PlayerData.flamesCollected), 6);
            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerSmall), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerMed), true);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerSmall), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerMed), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.grimmChildLevel), 2);
            SceneDataUtil.Save("Mines_10", "Flamebearer Spawn");
            SceneDataUtil.Save("Ruins1_28", "Flamebearer Spawn");
            SceneDataUtil.Save("Fungus1_10", "Flamebearer Spawn");
            SceneDataUtil.Save("Tutorial_01", "Flamebearer Spawn");
            SceneDataUtil.Save("RestingGrounds_06", "Flamebearer Spawn");
            SceneDataUtil.Save("Deepnest_East_03", "Flamebearer Spawn");
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(nameof(PlayerData.instance.gotCharm_40));
        }
    }
}
