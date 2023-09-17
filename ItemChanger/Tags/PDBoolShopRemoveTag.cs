namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which indicates that an item should be removed from the shop's stock if the PlayerData bool evaluates to the specified value.
    /// </summary>
    [ItemTag]
    public class PDBoolShopRemoveTag : Tag, IShopRemovalTag
    {
        public string fieldName;
        public bool removeVal = true;

        public bool Remove => PlayerData.instance.GetBool(fieldName) == removeVal;
    }
}
