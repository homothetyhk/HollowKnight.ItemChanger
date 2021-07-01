using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger
{
    [Flags]
    public enum MessageType
    {
        None = 0,
        Corner = 1,
        Big = 2,
        Any = Corner | Big,
    }

    public interface IUIDef
    {
        void SendMessage(MessageType type, Action callback);
        string GetPreviewName();
        string GetShopDesc();
        Sprite GetSprite();
        string GetPostviewName();
    }
}
