namespace ItemChanger
{
    /// <summary>
    /// Tag used for carrying information about costs, particularly for items in a shop or other multicost placement.
    /// </summary>
    public class CostTag : Tag
    {
        public Cost Cost { get; set; }
        public override void Load(object parent)
        {
            base.Load(parent);
            Cost?.Load();
        }
        public override void Unload(object parent)
        {
            base.Unload(parent);
            Cost?.Unload();
        }
    }
}
