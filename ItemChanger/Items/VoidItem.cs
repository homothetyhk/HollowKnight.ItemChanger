using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which does nothing.
    /// </summary>
    public class VoidItem : AbstractItem
    {
        public static VoidItem Nothing => new VoidItem()
        {
            name = "Nothing",
            UIDef = new UIDefs.MsgUIDef
            {
                name = new BoxedString { Value = "Nothing" },
                shopDesc = new BoxedString { Value = "" },
                sprite = new EmptySprite(),
            }
        };

        public override void GiveImmediate(GiveInfo info)
        {
            return;
        }
    }
}
