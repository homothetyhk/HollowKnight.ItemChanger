namespace ItemChanger.Items
{
    public class NailArtItem : BoolItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            PlayerData.instance.SetBool(nameof(PlayerData.hasNailArt), true);
            if (PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone)) 
                && PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                && PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.hasAllNailArts), true);
            }
        }
    }
}
