namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which sets a PlayerData bool when loaded.
    /// </summary>
    public class SetPDBoolOnLoadTag : Tag
    {
        public string boolName;
        public bool value = true;

        public override void Load(object parent)
        {
            base.Load(parent);
            PlayerData.instance.SetBool(boolName, value);
        }
    }
}
