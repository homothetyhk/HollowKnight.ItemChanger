using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class EquippedCharmItem : AbstractItem
    {
        public int charmNum;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);
            PlayerData.instance.SetBool($"gotCharm_{charmNum}", true);
            PlayerData.instance.IncrementInt(nameof(PlayerData.charmsOwned));
            PlayerData.instance.SetBool($"equippedCharm_{charmNum}", true);
            PlayerData.instance.EquipCharm(charmNum);

            PlayerData.instance.CalculateNotchesUsed();
            if (PlayerData.instance.GetInt(nameof(PlayerData.charmSlotsFilled)) > PlayerData.instance.GetInt(nameof(PlayerData.charmSlots)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.overcharmed), true);
            }
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool($"gotCharm_{charmNum}");
        }
    }
}
