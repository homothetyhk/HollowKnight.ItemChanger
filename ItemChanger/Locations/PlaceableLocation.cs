using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    public abstract class PlaceableLocation : ContainerLocation
    {
        /// <summary>
        /// An auxillary ContainerLocation receives its container through PlaceContainer during ActiveSceneChanged, rather than by requesting it in GetPrimaryContainer.
        /// </summary>
        public bool auxillary;

        public abstract void PlaceContainer(GameObject obj, Container containerType);
    }
}
