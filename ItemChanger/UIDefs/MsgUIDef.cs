using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.UIDefs
{
    public class MsgUIDef : UIDef
    {
        public IString name;
        public IString shopDesc;
        public ISprite sprite;

        public override string GetPostviewName()
        {
            return name.Value;
        }

        public override string GetPreviewName()
        {
            return name.Value;
        }

        public override string GetShopDesc()
        {
            return shopDesc.Value;
        }

        public override Sprite GetSprite()
        {
            return sprite.Value;
        }

        public override void SendMessage(MessageType type, Action callback)
        {
            ItemChangerMod.instance.Log($"{GetPostviewName()}/{GetType().Name}");
            if ((type & MessageType.Corner) == MessageType.Corner)
            {
                Internal.MessageController.Enqueue(GetSprite(), (this as UIDef).GetPostviewName());
            }

            callback?.Invoke();
        }

        // Remember that Clone is not memberwise, so it must be overridden in any descendent.
        public override UIDef Clone()
        {
            return new MsgUIDef
            {
                name = name.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone()
            };
        }
    }
}
