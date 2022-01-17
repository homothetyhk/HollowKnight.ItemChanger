using Newtonsoft.Json;

namespace ItemChanger.Items
{
    /// <summary>
    /// CharmItem which gives the unbreakable version of the charm.
    /// </summary>
    public class UnbreakableCharmItem : CharmItem
    {
        public string GetUnbreakableBool() => charmNum switch
        {
            23 => nameof(PlayerData.fragileHealth_unbreakable),
            24 => nameof(PlayerData.fragileGreed_unbreakable),
            25 => nameof(PlayerData.fragileStrength_unbreakable),
            _ => throw new ArgumentException("CharmNum out of range."),
        };

        public string GetBrokenBool() => charmNum switch
        {
            23 => nameof(PlayerData.brokenCharm_23),
            24 => nameof(PlayerData.brokenCharm_24),
            25 => nameof(PlayerData.brokenCharm_25),
            _ => throw new ArgumentException("CharmNum out of range."),
        };

        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            if (PlayerData.instance.GetBool(GetBrokenBool())) PlayerData.instance.SetBool(GetBrokenBool(), false);
            PlayerData.instance.SetBool(GetUnbreakableBool(), true);
        }

        public override bool Redundant()
        {
            return base.Redundant() && PlayerData.instance.GetBool(GetUnbreakableBool());
        }
    }
}
