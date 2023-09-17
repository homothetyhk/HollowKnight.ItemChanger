namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag applied to a charm item so that the charm is equipped when given.
    /// </summary>
    [TagConstrainedTo<Items.CharmItem>]
    public class EquipCharmOnGiveTag : Tag
    {
        private int charmNum;
        private string equipBool => $"equippedCharm_{charmNum}";

        public override void Load(object parent)
        {
            base.Load(parent);
            Items.CharmItem charm = (Items.CharmItem)parent;
            charmNum = charm.charmNum;
            charm.AfterGive += AfterGiveItem;
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);
            Items.CharmItem charm = (Items.CharmItem)parent;
            charm.AfterGive -= AfterGiveItem;
        }

        public void AfterGiveItem(ReadOnlyGiveEventArgs args)
        {
            if (PlayerData.instance.GetBool(equipBool)) return;

            PlayerData.instance.SetBool(equipBool, true);
            PlayerData.instance.EquipCharm(charmNum);

            PlayerData.instance.CalculateNotchesUsed();
            if (PlayerData.instance.GetInt(nameof(PlayerData.charmSlotsFilled)) > PlayerData.instance.GetInt(nameof(PlayerData.charmSlots)))
            {
                PlayerData.instance.SetBool(nameof(PlayerData.overcharmed), true);
            }

            PlayerData.instance.CountCharms();
        }
    }
}
