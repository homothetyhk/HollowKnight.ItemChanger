using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Items
{
    public class JournalEntryItem : AbstractItem
    {
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
            string boolName2 = "newData" + playerDataName;
            return PlayerData.instance.GetBool(boolName) && PlayerData.instance.GetBool(boolName2) && PlayerData.instance.GetInt(intName) <= 0;
        }
    }
}
