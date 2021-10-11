using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Interface used when ItemChanger checks tags to determine whether an item is persistent or semipersistent, and should be refreshed.
    /// </summary>
    interface IPersistenceTag
    {
        Persistence Persistence { get; }
    }
}
