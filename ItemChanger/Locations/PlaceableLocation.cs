using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Base type for ContainerLocations which support receiving receiving a container at a placement-controlled time and manner.
    /// </summary>
    public abstract class PlaceableLocation : ContainerLocation
    {
        /// <summary>
        /// A managed ContainerLocation receives its container through PlaceContainer, rather than by requesting it in GetContainer.
        /// </summary>
        ///
        public bool managed;

        public abstract void PlaceContainer(GameObject obj, string containerType);
    }
}
