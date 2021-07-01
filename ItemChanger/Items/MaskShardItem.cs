using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class MaskShardItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            for (int i = 0; i < amount; i++) GiveMaskShard();
        }

        public static void GiveMaskShard()
        {
            PlayerData.instance.SetBool(nameof(PlayerData.heartPieceCollected), true);
            PlayerData.instance.IncrementInt(nameof(PlayerData.heartPieces));

            if (PlayerData.instance.GetInt(nameof(PlayerData.heartPieces)) >= 4)
            {
                HeroController.instance.AddToMaxHealth(1);
                PlayMakerFSM.BroadcastEvent("MAX HP UP");
                PlayMakerFSM.BroadcastEvent("HERO HEALED FULL");
            }

            if (PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) == PlayerData.instance.GetInt(nameof(PlayerData.maxHealthCap)))
            {
                PlayerData.instance.SetInt(nameof(PlayerData.heartPieces), 4);
            }
            else PlayerData.instance.IntAdd(nameof(PlayerData.heartPieces), -4);
        }

    }
}
