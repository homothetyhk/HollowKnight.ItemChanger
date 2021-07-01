using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.UIDefs
{
    public class MinUIDef : IUIDef
    {
        public string name;
        public string desc;
        public Sprite sprite;

        public void SendMessage(MessageType messageType, Action callback)
        {
            if ((messageType & MessageType.Corner) == MessageType.Corner)
            {
                MessageController.Enqueue(sprite, name);
            }

            callback?.Invoke();
        }
        public Sprite GetSprite() => sprite;
        public string GetShopDesc() => desc;
        public string GetPreviewName() => name;
        public string GetPostviewName() => name;
    }
}
