using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.UIDefs
{
    public class UIDef : IUIDef
    {
        public string spriteKey;
        public string nameKey;
        public string shopDescKey;

        public virtual void SendMessage(MessageType type, Action callback = null)
        {
            if ((type & MessageType.Corner) == MessageType.Corner)
            {
                MessageController.Enqueue(SpriteManager.GetSprite(spriteKey), Language.Language.Get(nameKey, "UI"));
            }

            callback?.Invoke();
        }

        public virtual string GetPostviewName() => Language.Language.Get(nameKey, "UI");

        public string GetPreviewName()
        {
            return GetPostviewName();
        }

        public string GetShopDesc()
        {
            return Language.Language.Get(shopDescKey, "UI").Replace("<br>", "\n");
        }

        public Sprite GetSprite()
        {
            return SpriteManager.GetSprite(spriteKey);
        }
    }
}
