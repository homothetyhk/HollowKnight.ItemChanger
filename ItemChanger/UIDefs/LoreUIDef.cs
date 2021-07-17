using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.UIDefs
{
    public class LoreUIDef : MsgUIDef
    {
        public IString lore;
        public TextType textType;

        public override void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Lore) == MessageType.Lore)
            {
                DialogueCenter.SendLoreMessage(lore.Value, callback, textType);
            }
            else base.SendMessage(type, callback);
        }

        public override UIDef Clone()
        {
            return new LoreUIDef
            {
                name = name.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone(),
                lore = lore.Clone(),
                textType = textType,
            };
        }
    }
}
