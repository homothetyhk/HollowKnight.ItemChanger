namespace ItemChanger.Items
{
    /// <summary>
    /// Item which sets all of the flags triggered when the corresponding dreamer would be obtained.
    /// </summary>
    public class DreamerItem : AbstractItem
    {
        public enum DreamerType
        {
            None,
            Lurien,
            Monomon,
            Herrah
        }
        public DreamerType dreamer;

        public override void GiveImmediate(GiveInfo info)
        {
            switch (dreamer)
            {
                case DreamerType.Lurien:
                    PlayerData.instance.SetBool(nameof(PlayerData.lurienDefeated), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.maskBrokenLurien), true);
                    break;
                case DreamerType.Monomon:
                    PlayerData.instance.SetBool(nameof(PlayerData.monomonDefeated), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.maskBrokenMonomon), true);
                    break;
                case DreamerType.Herrah:
                    PlayerData.instance.SetBool(nameof(PlayerData.hegemolDefeated), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.maskBrokenHegemol), true);
                    break;
            }
            if (PlayerData.instance.GetInt(nameof(PlayerData.guardiansDefeated)) == 0)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.hornetFountainEncounter), true);
                PlayerData.instance.SetBool(nameof(PlayerData.marmOutside), true);
                PlayerData.instance.SetBool(nameof(PlayerData.crossroadsInfected), true);
            }
            if (PlayerData.instance.GetInt(nameof(PlayerData.guardiansDefeated)) == 2)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.dungDefenderSleeping), true);
                PlayerData.instance.IncrementInt(nameof(PlayerData.brettaState));
                PlayerData.instance.IncrementInt(nameof(PlayerData.mrMushroomState));
                PlayerData.instance.SetBool(nameof(PlayerData.corniferAtHome), true);
                PlayerData.instance.SetBool(nameof(PlayerData.metIselda), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_cityLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_abyssLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_cliffsLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_crossroadsLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_deepnestLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_fogCanyonLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_fungalWastesLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_greenpathLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_minesLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_outskirtsLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_royalGardensLeft), true);
                PlayerData.instance.SetBool(nameof(PlayerData.corn_waterwaysLeft), true);
            }
            if (PlayerData.instance.guardiansDefeated < 3)
            {
                PlayerData.instance.IncrementInt(nameof(PlayerData.guardiansDefeated));
            }
        }
    }
}
