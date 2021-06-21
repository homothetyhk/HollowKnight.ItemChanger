using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class VoidItem : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            return;
        }
    }
}
