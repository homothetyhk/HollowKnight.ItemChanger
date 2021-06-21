using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class LifebloodItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            for (int i = 0; i < amount; i++)
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
            }
        }
    }
}
