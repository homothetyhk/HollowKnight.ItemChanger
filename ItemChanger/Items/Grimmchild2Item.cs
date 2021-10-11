using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            PlayerData.instance.SetInt(nameof(PlayerData.flamesCollected), 3);
            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerSmall), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedFlameBearerMed), true);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerSmall), 3);
            PlayerData.instance.SetInt(nameof(PlayerData.killsFlameBearerMed), 3);
            PlayerData.instance.SetInt(nameof(PlayerData.grimmChildLevel), 2);
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Mines_10",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Ruins1_28",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Fungus1_10",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Tutorial_01",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "RestingGrounds_06",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                sceneName = "Deepnest_East_03",
                id = "Flamebearer Spawn",
                activated = true,
                semiPersistent = false
            });
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(nameof(PlayerData.instance.gotCharm_40));
        }
    }
}
