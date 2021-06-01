using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Components;

namespace ItemChanger.UIDefs
{
    public class BigUIDef : UIDef
    {
        public string bigSpriteKey;
        public string takeKey;
        public string buttonKey;
        public string descOneKey;
        public string descTwoKey;

        public override void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Big) == MessageType.Big)
            {
                BigItemPopup.Show(SpriteManager.GetSprite(bigSpriteKey), takeKey, nameKey, buttonKey, descOneKey, descTwoKey, callback);
            }
            else base.SendMessage(type, callback);
        }
    }
}
