using ItemChanger.Internal;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which plays a lore sound when the context does not support sending up a lore prompt.
    /// </summary>
    public class LoreItem : AbstractItem
    {
        public string loreSheet;
        public string loreKey;
        public TextType textType;

        public override void GiveImmediate(GiveInfo info)
        {
            if ((info.MessageType & MessageType.Lore) == MessageType.Lore) return;
            SoundManager.Instance.PlayClipAtPoint("LoreSound",
                info.Transform != null ? info.Transform.position
                : HeroController.instance != null ? HeroController.instance.transform.position
                : Camera.main.transform.position + 2 * Vector3.up);
        }

    }
}
