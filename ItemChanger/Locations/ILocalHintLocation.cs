using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Interface for locations which support a nearby toggleable item preview.
    /// </summary>
    public interface ILocalHintLocation
    {
        bool HintActive { get; set; }
    }
}
