namespace ItemChanger.Items
{
    /// <summary>
    /// Item which fully unlocks and completes the corresponding entry of the Hunter's Journal.
    /// </summary>
    public class JournalEntryItem : AbstractItem
    {
        /// <summary>
        /// If the journal entry corresponds to fields "killed{name}", "kills{name}", and "newData{name}", then this field is "{name}"
        /// </summary>
        public string playerDataName;

        public override void GiveImmediate(GiveInfo info)
        {
            string boolName = "killed" + playerDataName;
            string intName = "kills" + playerDataName;
            string boolName2 = "newData" + playerDataName;
            PlayerData.instance.SetBool(boolName, true);
            PlayerData.instance.SetBool(boolName2, true);
            PlayerData.instance.SetInt(intName, 0);
        }

        public override bool Redundant()
        {
            string boolName = "killed" + playerDataName;
            string intName = "kills" + playerDataName;
            return PlayerData.instance.GetBool(boolName) && PlayerData.instance.GetInt(intName) <= 0;
        }
    }
}
