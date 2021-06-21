using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class SoulItem : AbstractItem
    {
        public int soul;

        public override void GiveImmediate(GiveInfo info)
        {
            HeroController.instance.AddMPCharge(soul);
        }
    }
}
