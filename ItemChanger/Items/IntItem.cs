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

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            PlayerData.instance.IntAdd(fieldName, amount);
        }
    }
}
