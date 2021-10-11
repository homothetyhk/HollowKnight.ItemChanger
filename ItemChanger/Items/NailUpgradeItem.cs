using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item designed to be compatible with Nailsmith upgrades and quest.
    /// </summary>
    public class NailUpgradeItem : AbstractItem
    {
        public int amount = 4;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.honedNail), true);
            PlayerData.instance.IntAdd(nameof(PlayerData.nailDamage), amount);
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            GameManager.instance.IncrementPlayerDataInt(nameof(PlayerData.nailSmithUpgrades));
            if (PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) == 4)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.nailsmithCliff), true);
            }
        }
    }
}
