namespace ItemChanger.Items
{
    /// <summary>
    /// Item which sets a PlayerData bool to the specified value.
    /// </summary>
    public class BoolItem : AbstractItem
    {
        public string fieldName;
        public bool setValue = true;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(fieldName, setValue);
        }
        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(fieldName);
        }
    }
}
