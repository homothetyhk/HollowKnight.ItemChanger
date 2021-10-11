using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the specified amount of essence.
    /// </summary>
    public class EssenceItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.IntAdd(nameof(PlayerData.dreamOrbs), amount);
            EventRegister.SendEvent("DREAM ORB COLLECT");
        }
    }
}
