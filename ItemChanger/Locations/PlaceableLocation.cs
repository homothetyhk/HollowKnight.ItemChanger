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
        /// A managed ContainerLocation receives its container through PlaceContainer during ActiveSceneChanged,
        /// rather than by requesting it in GetContainer.
        /// </summary>
        public bool managed;

        public abstract void PlaceContainer(GameObject obj, string containerType);
    }
}
