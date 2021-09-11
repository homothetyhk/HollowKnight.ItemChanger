using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ItemChanger.Internal;

namespace ItemChanger.UIDefs
{
    public class GrubUIDef : MsgUIDef
    {
        public override string GetPostviewName()
        {
            return $"A Grub! ({PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected))}/46)";
        }

        public override UIDef Clone()
        {
            return new GrubUIDef
            {
                name = name.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone(),
            };
        }
    }
}
