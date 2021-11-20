namespace ItemChanger.Components
{
    /// <summary>
    /// A component which is specially handled by ShopLocation to be used as a shop item.
    /// </summary>
    public class ModShopItemStats : MonoBehaviour
    {
        public AbstractItem item;
        public AbstractPlacement placement;
        public UIDef UIDef => item.GetResolvedUIDef(placement);
        public Cost cost
        {
            get => item.GetTag<CostTag>()?.Cost;
            set => item.GetOrAddTag<CostTag>().Cost = cost;
        }
    }
}
