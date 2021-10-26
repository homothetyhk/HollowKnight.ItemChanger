using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for a location or placement to indicate that a container is not supported and should not be chosen.
    /// </summary>
    public class UnsupportedContainerTag : Tag
    {
        public string containerType;
    }
}
