using ItemChanger.Internal;
using Newtonsoft.Json;

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

        public BoxedSprite(Sprite Value) => this.Value = Value;

        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    /// <summary>
    /// An ISprite which retrieves its sprite from a SpriteManager.
    /// </summary>
    public abstract class EmbeddedSprite : ISprite
    {
        public string key;
        [JsonIgnore] public abstract SpriteManager SpriteManager { get; }
        [JsonIgnore] public Sprite Value => SpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    /// <summary>
    /// An EmbeddedSprite which retrieves its sprite from SpriteManager.Instance.
    /// </summary>
    [Serializable]
    public class ItemChangerSprite : EmbeddedSprite
    {
        public ItemChangerSprite(string key)
        {
            this.key = key;
        }

        public override SpriteManager SpriteManager => SpriteManager.Instance;
    }

    [Serializable]
    public class EmptySprite : ISprite
    {
        [JsonIgnore] public Sprite Value => Modding.CanvasUtil.NullSprite();
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    [Serializable]
    public class DualSprite : ISprite
    {
        public IBool Test;
        public ISprite TrueSprite;
        public ISprite FalseSprite;

        public DualSprite(IBool Test, ISprite TrueSprite, ISprite FalseSprite)
        {
            this.Test = Test;
            this.TrueSprite = TrueSprite;
            this.FalseSprite = FalseSprite;
        }

        [JsonIgnore] public Sprite Value => Test.Value ? TrueSprite.Value : FalseSprite.Value;
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
