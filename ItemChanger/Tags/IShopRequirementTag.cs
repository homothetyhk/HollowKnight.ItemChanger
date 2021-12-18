namespace ItemChanger.Tags
{
    /// <summary>
    /// Interface for tags used by shops to determine special conditions which must be met before an item appears in stock.
    /// </summary>
    public interface IShopRequirementTag
    {
        bool MeetsRequirement { get; }
    }
}
