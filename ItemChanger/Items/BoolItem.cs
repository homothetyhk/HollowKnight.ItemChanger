using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
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
