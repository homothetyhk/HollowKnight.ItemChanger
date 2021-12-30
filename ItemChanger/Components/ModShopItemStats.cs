namespace ItemChanger.Components
{
    /// <summary>
    /// A component which is specially handled by ShopLocation to be used as a shop item.
    /// </summary>
    public class ModShopItemStats : MonoBehaviour
    {
        public AbstractItem item;
        public AbstractPlacement placement;
        public bool IsSecretItem()
        {
            return item.HasTag<Tags.DisableItemPreviewTag>()
                || (placement != null && placement.HasTag<Tags.DisableItemPreviewTag>());
        }
        public Sprite GetSprite() => item.GetPreviewSprite(placement);
        public string GetPreviewName() => item.GetPreviewName(placement);
        public string GetShopDesc()
        {
            if (item.HasTag<Tags.DisableItemPreviewTag>()
                || (placement != null && placement.HasTag<Tags.DisableItemPreviewTag>())) return "???";
            UIDef def = item.GetResolvedUIDef(placement);
            return def?.GetShopDesc() ?? "???";
        }

        public string GetShopCostText()
        {
            if (item.HasTag<Tags.DisableCostPreviewTag>()
                || (placement != null && placement.HasTag<Tags.DisableCostPreviewTag>()))
                return "The cost of this item is a secret!";
            return cost.GetShopCostText();
        }

        public string GetRecordText()
        {
            string text = GetPreviewName();
            if (item.HasTag<Tags.DisableCostPreviewTag>()
                || (placement != null && placement.HasTag<Tags.DisableCostPreviewTag>()))
            {
                text += "  -  The cost of this item is a secret!";
            }
            else if (cost is not null && !cost.Paid)
            {
                text += $"  -  {cost.GetCostText()}";
            }
            return text;
        }

        public Cost cost
        {
            get => item.GetTag<CostTag>()?.Cost;
            set => item.GetOrAddTag<CostTag>().Cost = cost;
        }
    }
}
