namespace ItemChanger.Items
{
    /// <summary>
    /// Item which increments a PlayerData field by the specified amount.
    /// </summary>
    public class IntItem : AbstractItem
    {
        public string fieldName;
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.IntAdd(fieldName, amount);
        }
    }
}
