using ItemChanger.Util;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives Grimmchild, activates the Nightmare Lantern, and auto-completes the first two flame collection quests.
    /// </summary>
    public class Grimmchild2Item : CharmItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternAppeared), true);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternLit), true);
            PlayerData.instance.SetBool(nameof(PlayerData.troupeInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.divineInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.metGrimm), true);
            PlayerData.instance.SetInt(nameof(PlayerData.flamesRequired), 3);
            // Skip first two collection quests
            PlayerData.instance.IntAdd(nameof(PlayerData.flamesCollected), 3);
            PlayerData.instance.IntAdd(nameof(PlayerData.flamesCollected), -3); // simulate paying for first upgrade, for GrimmkinFlameManager if present
            PlayerData.instance.IntAdd(nameof(PlayerData.flamesCollected), 3);

            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerSmall), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerMed), true);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerSmall), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerMed), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.grimmChildLevel), 2);
            SceneDataUtil.Save(SceneNames.Mines_10, "Flamebearer Spawn");
            SceneDataUtil.Save(SceneNames.Ruins1_28, "Flamebearer Spawn");
            SceneDataUtil.Save(SceneNames.Fungus1_10, "Flamebearer Spawn");
            SceneDataUtil.Save(SceneNames.Tutorial_01, "Flamebearer Spawn");
            SceneDataUtil.Save(SceneNames.RestingGrounds_06, "Flamebearer Spawn");
            SceneDataUtil.Save(SceneNames.Deepnest_East_03, "Flamebearer Spawn");
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(gotBool);
        }
    }
}
