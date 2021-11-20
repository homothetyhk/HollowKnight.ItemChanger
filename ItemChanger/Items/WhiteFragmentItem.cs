namespace ItemChanger.Items
{
    /// <summary>
    /// Item which sets all of the flags associated to its royalCharmLevel (e.g. L/R White Fragment, Kingsoul, Void Heart)
    /// </summary>
    public class WhiteFragmentItem : AbstractItem
    {
        public int royalCharmLevel;

        public override void GiveImmediate(GiveInfo info)
        {
            if (!PlayerData.instance.GetBool(nameof(PlayerData.gotCharm_36)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.gotCharm_36), true);
                PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);
            }

            PlayerData.instance.SetInt(nameof(PlayerData.royalCharmState), royalCharmLevel);

            if (royalCharmLevel == 4)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.gotShadeCharm), true);
                PlayerData.instance.SetInt(nameof(PlayerData.charmCost_36), 0);
                PlayerData.instance.SetBool(nameof(PlayerData.equippedCharm_36), true);
                if (!PlayerData.instance.equippedCharms.Contains(36)) PlayerData.instance.equippedCharms.Add(36);
            }

            PlayerData.instance.CountCharms();
        }

        public override bool Redundant()
        {
            int royalCharmState = PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState));

            if (royalCharmLevel < 3) return royalCharmState > 0;
            else return royalCharmState >= royalCharmLevel;
        }
    }
}
