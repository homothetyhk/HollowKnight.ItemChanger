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
        public string descKey;

        public virtual void SendMessage(MessageType type, Action callback = null)
        {
            if ((type & MessageType.Corner) == MessageType.Corner)
            {
                /*
                GameObject popup = ObjectCache.RelicGetMsg;
                popup.transform.Find("Text").GetComponent<TMPro.TextMeshPro>().text = Language.Language.Get(nameKey, "UI");
                popup.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSprite(spriteKey);
                popup.SetActive(true);
                */
                MessageController.Enqueue(SpriteManager.GetSprite(spriteKey), Language.Language.Get(nameKey, "UI"));
            }

            callback?.Invoke();
        }

        public virtual void GetShopData(out Sprite shopSprite, out string nameKey, out string descKey)
        {
            shopSprite = SpriteManager.GetSprite(spriteKey);
            nameKey = this.nameKey;
            descKey = this.descKey;
        }

        public virtual string GetDisplayName() => Language.Language.Get(nameKey, "UI");
    }
}
