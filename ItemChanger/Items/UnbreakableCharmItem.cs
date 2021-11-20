namespace ItemChanger.Items
{
    /// <summary>
    /// CharmItem which gives the unbreakable version of the charm.
    /// </summary>
    public class UnbreakableCharmItem : CharmItem
    {
        public string unbreakableBool
        {
            get
            {
                switch (charmNum)
                {
                    case 23:
                        return nameof(PlayerData.fragileHealth_unbreakable);
                    case 24:
                        return nameof(PlayerData.fragileGreed_unbreakable);
                    case 25:
                        return nameof(PlayerData.fragileStrength_unbreakable);
                    default:
                        throw new ArgumentException("CharmNum out of range.");
                }
            }
        }

        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            PlayerData.instance.SetBool(unbreakableBool, true);
        }

        public override bool Redundant()
        {
            return base.Redundant() && PlayerData.instance.GetBool(unbreakableBool);
        }
    }
}
