using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;
using ItemChanger.Internal;
using GlobalEnums;

namespace ItemChanger.UIDefs
{
    /// <summary>
    /// UIDef which produces a full-screen message using BigItemPopup if possible, otherwise defaulting to the action of MsgUIDef.
    /// </summary>
    public class BigUIDef : MsgUIDef
    {
        public ISprite bigSprite;
        public IString take;
        public IString press;
        public HeroActionButton? heroAction;
        public IString descOne;
        public IString descTwo;

        public override void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Big) == MessageType.Big)
            {
                BigItemPopup.Show(
                    bigSprite.GetValue(),
                    take?.GetValue()?.Replace('\n', ' '),
                    GetPostviewName(),
                    press?.GetValue()?.Replace('\n', ' '),
                    heroAction,
                    descOne?.GetValue()?.Replace('\n', ' '),
                    descTwo?.GetValue()?.Replace('\n', ' '),
                    callback);
            }
            else base.SendMessage(type, callback);
        }

        public override UIDef Clone()
        {
            return new BigUIDef
            {
                name = name?.Clone(),
                shopDesc = shopDesc?.Clone(),
                sprite = sprite?.Clone(),
                bigSprite = bigSprite?.Clone(),
                press = press?.Clone(),
                heroAction = heroAction,
                take = take?.Clone(),
                descOne = descOne?.Clone(),
                descTwo = descTwo?.Clone()
            };
        }
    }
}
