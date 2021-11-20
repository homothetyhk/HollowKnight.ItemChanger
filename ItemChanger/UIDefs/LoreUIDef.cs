using ItemChanger.Internal;

namespace ItemChanger.UIDefs
{
    /// <summary>
    /// UIDef which starts a text conversation with the specified text and type, otherwise defaulting to the action of MsgUIDef.
    /// </summary>
    public class LoreUIDef : MsgUIDef
    {
        public IString lore;
        public TextType textType;

        public override void SendMessage(MessageType type, Action callback)
        {
            if ((type & MessageType.Lore) == MessageType.Lore)
            {
                DialogueCenter.SendLoreMessage(lore.GetValue(), callback, textType);
            }
            else base.SendMessage(type, callback);
        }

        public override UIDef Clone()
        {
            return new LoreUIDef
            {
                name = name.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone(),
                lore = lore.Clone(),
                textType = textType,
            };
        }
    }
}
