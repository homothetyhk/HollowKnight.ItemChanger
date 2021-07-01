using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    interface IPersistenceTag
    {
        Persistence Persistence { get; }
    }
}
