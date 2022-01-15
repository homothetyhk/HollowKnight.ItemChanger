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

    [Serializable]
    public class ItemChangerSprite : ISprite
    {
        public string key;

        public ItemChangerSprite(string key)
        {
            this.key = key;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => Internal.SpriteManager.Instance.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    [Serializable]
    public class EmptySprite : ISprite
    {
        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => Modding.CanvasUtil.NullSprite();
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

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => Test.Value ? TrueSprite.Value : FalseSprite.Value;
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
