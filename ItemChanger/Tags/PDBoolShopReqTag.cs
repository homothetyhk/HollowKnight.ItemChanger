namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which indicates that an item should only appear in the shop's stock if the PlayerData bool evaluates to the specified value.
    /// </summary>
    public class PDBoolShopReqTag : Tag, IShopRequirementTag
    {
        public string fieldName;
        public bool reqVal = true;

        public bool MeetsRequirement => PlayerData.instance.GetBool(fieldName) == reqVal;
    }
}
