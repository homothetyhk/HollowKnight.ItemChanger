using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Items
{
    public class SoulTotemItem : SoulItem
    {
        public SoulTotemSubtype soulTotemSubtype;
        /// <summary>
        /// The number of hits to contribute to the soul totem. A negative number will result in an infinite totem.
        /// </summary>
        public int hitCount;

        public override string GetPreferredContainer() => Container.Totem;

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.Container != Container.Totem)
            {
                base.GiveImmediate(info);
            }
        }
    }
}
