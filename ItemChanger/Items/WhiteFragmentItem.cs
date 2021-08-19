using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class WhiteFragmentItem : AbstractItem
    {
        public int royalCharmLevel;

        public override void GiveImmediate(GiveInfo info)
        {
            if (!PlayerData.instance.GetBool(nameof(PlayerData.gotCharm_36)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.gotCharm_36), true);
                PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);
            }

            PlayerData.instance.SetInt(nameof(PlayerData.royalCharmState), royalCharmLevel);

            if (royalCharmLevel == 4)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.gotShadeCharm), true);
                PlayerData.instance.SetInt(nameof(PlayerData.charmCost_36), 0);
                PlayerData.instance.EquipCharm(36);
                PlayerData.instance.SetBool(nameof(PlayerData.equippedCharm_36), true);
                if (!PlayerData.instance.equippedCharms.Contains(36)) PlayerData.instance.equippedCharms.Add(36);
            }

            PlayerData.instance.CountCharms();
        }

        public override bool Redundant()
        {
            int royalCharmState = PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState));

            if (royalCharmLevel < 3) return royalCharmState > 0;
            else return royalCharmState >= royalCharmLevel;
        }
    }
}
