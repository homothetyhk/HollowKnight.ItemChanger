using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.UIDefs
{
    /// <summary>
    /// A MsgUIDef with distinct preview and postview names.
    /// </summary>
    public class SplitUIDef : MsgUIDef
    {
        public IString preview;
        public override string GetPreviewName()
        {
            return preview.GetValue();
        }
        public override UIDef Clone()
        {
            return new SplitUIDef
            {
                name = name.Clone(),
                preview = preview.Clone(),
                shopDesc = shopDesc.Clone(),
                sprite = sprite.Clone()
            };
        }
    }
}
