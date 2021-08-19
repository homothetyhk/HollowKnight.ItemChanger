using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class EquippedCharmItem : AbstractItem
    {
        public int charmNum;

        public string gotBool => $"gotCharm_{charmNum}";
        public string equipBool => $"equippedCharm_{charmNum}";

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);
            PlayerData.instance.SetBool(gotBool, true);
            PlayerData.instance.SetBool(equipBool, true);
            PlayerData.instance.EquipCharm(charmNum);

            PlayerData.instance.CalculateNotchesUsed();
            if (PlayerData.instance.GetInt(nameof(PlayerData.charmSlotsFilled)) > PlayerData.instance.GetInt(nameof(PlayerData.charmSlots)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.overcharmed), true);
            }

            PlayerData.instance.CountCharms();
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(gotBool);
        }
    }
}
