namespace ItemChanger
{
    public abstract class UIDef
    {
        public abstract void SendMessage(MessageType type, Action callback = null);

        public abstract string GetPostviewName();
        public virtual string GetPreviewName()
        {
            return GetPostviewName();
        }

        public abstract string GetShopDesc();

        public abstract Sprite GetSprite();

        public virtual UIDef Clone()
        {
            return (UIDef)MemberwiseClone();
        }
    }
}
