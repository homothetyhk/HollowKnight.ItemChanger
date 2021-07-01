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
                MessageController.Enqueue(SpriteManager.GetSprite(spriteKey), GetPostviewName());
            }
            callback?.Invoke();
        }

        public string GetPreviewName() => $"Grub";

        public string GetPostviewName() => "Grub";

        public string GetShopDesc() => Language.Language.Get("RANDOMIZER_DESC_GRUB", "UI");

        public Sprite GetSprite() => SpriteManager.GetSprite(spriteKey);

        public static GrubUIDef Def = new GrubUIDef();
    }
}
