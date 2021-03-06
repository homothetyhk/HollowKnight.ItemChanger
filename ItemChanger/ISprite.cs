﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger
{
    public interface ISprite
    {
        Sprite Value { get; }
        ISprite Clone();
    }

    public class ItemChangerSprite : ISprite
    {
        public string key;
        public Sprite Value => Internal.SpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    public class EmptySprite : ISprite
    {
        public Sprite Value => Modding.CanvasUtil.NullSprite();
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

}
