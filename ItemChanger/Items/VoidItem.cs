namespace ItemChanger.Items
{
    /// <summary>
    /// Item which does nothing.
    /// </summary>
    public class VoidItem : AbstractItem
    {
        public static VoidItem Nothing => new VoidItem()
        {
            name = "Nothing",
            UIDef = new UIDefs.MsgUIDef
            {
                name = new BoxedString("Nothing"),
                shopDesc = new BoxedString(""),
                sprite = new EmptySprite(),
            }
        };

        public override void GiveImmediate(GiveInfo info)
        {
            return;
        }
    }
}
