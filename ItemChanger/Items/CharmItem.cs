namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the charm with the corresponding charmNum.
    /// </summary>
    public class CharmItem : AbstractItem
    {
        public int charmNum;

        public string gotBool => $"gotCharm_{charmNum}";

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);
            PlayerData.instance.SetBool(gotBool, true);
            PlayerData.instance.CountCharms();
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(gotBool);
        }
    }
}
