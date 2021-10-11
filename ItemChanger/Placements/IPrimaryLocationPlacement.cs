using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Placements
{
    /// <summary>
    /// Interface for accessing the primary location of a placement, if it has one.
    /// </summary>
    public interface IPrimaryLocationPlacement
    {
        AbstractLocation Location { get; }
    }
}
