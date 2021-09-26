using System;
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

    /// <summary>
    /// ISprite wrapper for Sprite. Use only for items created and disposed at runtime--it is not serializable.
    /// </summary>
    public class BoxedSprite : ISprite
    {
        public Sprite Value { get; set; }
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    public class ItemChangerSprite : ISprite
    {
        public string key;

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => Internal.SpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    public class EmptySprite : ISprite
    {
        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => Modding.CanvasUtil.NullSprite();
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

}
