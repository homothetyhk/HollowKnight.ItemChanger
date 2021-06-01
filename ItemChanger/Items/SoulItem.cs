using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class SoulItem : AbstractItem
    {
        public int soul;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            HeroController.instance.AddMPCharge(soul);
        }
    }
}
