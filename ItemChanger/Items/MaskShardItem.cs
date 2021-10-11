using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the specified number of mask shards.
    /// </summary>
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

            while (PlayerData.instance.GetInt(nameof(PlayerData.heartPieces)) >= 4)
            {
                PlayMakerFSM.BroadcastEvent("HERO HEALED FULL");
                HeroController.instance.AddToMaxHealth(1);
                PlayMakerFSM.BroadcastEvent("MAX HP UP");

                if (PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) == PlayerData.instance.GetInt(nameof(PlayerData.maxHealthCap)))
                {
                    PlayerData.instance.SetInt(nameof(PlayerData.heartPieces), 0);
                    PlayerData.instance.SetBool(nameof(PlayerData.heartPieceMax), true);
                }
                else PlayerData.instance.IntAdd(nameof(PlayerData.heartPieces), -4);
            }
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) == PlayerData.instance.GetInt(nameof(PlayerData.maxHealthCap));
        }

    }
}
