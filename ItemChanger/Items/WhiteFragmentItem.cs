using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class WhiteFragmentItem : AbstractItem
    {
        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            int royalCharmState = PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState));

            if (royalCharmState < 4)
            {
                PlayerData.instance.IncrementInt(nameof(PlayerData.royalCharmState));
            }
            switch (royalCharmState)
            {
                case 1:
                    PlayerData.instance.SetBool(nameof(PlayerData.gotCharm_36), true);
                    PlayerData.instance.IncrementInt(nameof(PlayerData.charmsOwned));
                    break;
                case 2:
                    PlayerData.instance.IncrementInt(nameof(PlayerData.royalCharmState));
                    break;
                case 4:
                    PlayerData.instance.SetBool(nameof(PlayerData.gotShadeCharm), true);
                    PlayerData.instance.SetInt(nameof(PlayerData.charmCost_36), 0);
                    PlayerData.instance.EquipCharm(36);
                    PlayerData.instance.SetBool(nameof(PlayerData.equippedCharm_36), true);
                    if (!PlayerData.instance.equippedCharms.Contains(36)) PlayerData.instance.equippedCharms.Add(36);
                    break;
            }
        }

        // Not clear what the best choice is here
        public override bool Redundant()
        {
            return PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState)) == 4;
        }

    }
}
