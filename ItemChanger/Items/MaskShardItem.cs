using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class MaskShardItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.heartPieceCollected), true);
            int count = PlayerData.instance.GetInt(nameof(PlayerData.heartPieces)) + amount;
            for (; count > 3; count -= 4)
            {
                HeroController.instance.AddToMaxHealth(1);
                PlayMakerFSM.BroadcastEvent("MAX HP UP");
                PlayMakerFSM.BroadcastEvent("HERO HEALED FULL");
            }

            switch (count)
            {
                case 0 when PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) == PlayerData.instance.GetInt(nameof(PlayerData.maxHealthCap)):
                    PlayerData.instance.SetInt(nameof(PlayerData.heartPieces), 4);
                    break;
                default:
                    PlayerData.instance.SetInt(nameof(PlayerData.heartPieces), count);
                    break;
            }
        }
    }
}
