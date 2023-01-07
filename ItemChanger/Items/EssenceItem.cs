namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the specified amount of essence.
    /// </summary>
    public class EssenceItem : AbstractItem
    {
        public static EssenceItem MakeEssenceItem(int amount)
        {
            return new()
            {
                amount = amount,
                name = $"{amount}_Essence",
                UIDef = new UIDefs.MsgUIDef
                {
                    name = new BoxedString(string.Format(Language.Language.Get("ESSENCE", "Fmt"), amount)),
                    shopDesc = new LanguageString("UI", "ITEMCHANGER_DESC_ESSENCE"),
                    sprite = new ItemChangerSprite("ShopIcons.Essence"),
                },
            };
        }

        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.IntAdd(nameof(PlayerData.dreamOrbs), amount);
            EventRegister.SendEvent("DREAM ORB COLLECT");
        }
    }
}
