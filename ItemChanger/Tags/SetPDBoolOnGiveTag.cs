namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which adds setting a PlayerData bool as a side effect to an item.
    /// <br/>Be warned that this effect is tied to the tag's parent, regardless of how it is modified during Give.
    /// </summary>
    [Obsolete("Use SetIBoolOnGiveTag instead.")]
    public class SetPDBoolOnGiveTag : Tag
    {
        public string fieldName;
        public bool setValue = true;

        public override void Load(object parent)
        {
            base.Load(parent);
            AbstractItem item = (AbstractItem)parent;
            item.OnGive += OnGive;
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);
            AbstractItem item = (AbstractItem)parent;
            item.OnGive -= OnGive;
        }

        public void OnGive(ReadOnlyGiveEventArgs args)
        {
            PlayerData.instance.SetBool(fieldName, setValue);
        }
    }
}
