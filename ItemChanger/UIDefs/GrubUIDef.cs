using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.UIDefs
{
    public class GrubUIDef : IUIDef
    {
        public const string spriteKey = "ShopIcons.Grub";

        public void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Corner) == MessageType.Corner)
            {
                MessageController.Enqueue(SpriteManager.GetSprite(spriteKey), GetDisplayName());
            }
            callback?.Invoke();
        }

        public string GetDisplayName() => "Grub";

        public void GetShopData(out Sprite shopSprite, out string nameKey, out string descKey)
        {
            shopSprite = SpriteManager.GetSprite(spriteKey);
            nameKey = "RANDOMIZER_NAME_GRUB";
            descKey = "RANDOMIZER_DESC_GRUB";
        }

        public static GrubUIDef Def = new GrubUIDef();
    }
}
