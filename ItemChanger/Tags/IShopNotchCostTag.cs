namespace ItemChanger
{
    /// <summary>
    /// Interface used by shops to determine whether an item has a notch cost that should be displayed.
    /// <br />The notch cost of a CharmItem or EquippedCharmItem is displayed even if it does not have this tag.
    /// </summary>
    public interface IShopNotchCostTag
    {
        int GetNotchCost(AbstractItem item);
    }
}
