using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class VoidItem : AbstractItem
    {
        public static VoidItem Nothing => new VoidItem()
        {
            name = "Nothing",
            UIDef = new UIDefs.MinUIDef
            {
                name = "Nothing",
                desc = "",
                sprite = Modding.CanvasUtil.NullSprite(),
            }
        };

        public override void GiveImmediate(GiveInfo info)
        {
            return;
        }
    }
}
