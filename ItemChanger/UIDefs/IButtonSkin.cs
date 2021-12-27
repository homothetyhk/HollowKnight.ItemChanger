using GlobalEnums;

namespace ItemChanger.UIDefs
{
    public interface IButtonSkin
    {
        ButtonSkin Value { get; }
        IButtonSkin Clone();
    }

    public class HeroActionButtonSkin : IButtonSkin
    {
        public HeroActionButton button;

        public HeroActionButtonSkin(HeroActionButton button)
        {
            this.button = button;
        }

        [Newtonsoft.Json.JsonIgnore]
        public ButtonSkin Value => UIManager.instance.uiButtonSkins.GetButtonSkinFor(button);
        public IButtonSkin Clone() => (IButtonSkin)MemberwiseClone();
    }
}
