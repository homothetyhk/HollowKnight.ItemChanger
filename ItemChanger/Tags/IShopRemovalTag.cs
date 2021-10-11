using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Interface for tags used by shops to determine special conditions upon which an item should be removed from stock.
    /// </summary>
    interface IShopRemovalTag
    {
        bool Remove { get; }
    }
}
