using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
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
