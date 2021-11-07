using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Items
{
    public class NotchItem : IntItem
    {
        public bool refillHealth = true;

        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            if (refillHealth) PlayMakerFSM.BroadcastEvent("HERO HEALED FULL");
            GameManager.instance.RefreshOvercharm();
        }
    }
}
