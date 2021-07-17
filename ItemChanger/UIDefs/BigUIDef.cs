using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;
using ItemChanger.Internal;

namespace ItemChanger.UIDefs
{
    public class BigUIDef : MsgUIDef
    {
        public ISprite bigSprite;
        public IString take;
        public IString button;
        public IString descOne;
        public IString descTwo;

        public override void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Big) == MessageType.Big)
            {
                BigItemPopup.Show(
                    bigSprite.Value,
                    take.Value.Replace('\n', ' '),
                    GetPostviewName(),
                    button.Value.Replace('\n', ' '),
                    descOne.Value.Replace('\n', ' '),
                    descTwo.Value.Replace('\n', ' '),
                    callback);
            }
            else base.SendMessage(type, callback);
        }

        public override UIDef Clone()
        {
            return new BigUIDef
            {
                name = name.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone(),
                bigSprite = bigSprite.Clone(),
                button = button.Clone(),
                take = take.Clone(),
                descOne = descOne.Clone(),
                descTwo = descTwo.Clone()
            };
        }
    }
}
