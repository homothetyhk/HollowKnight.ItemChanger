using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Placements;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Location type which supports placing multiple kinds of objects.
    /// </summary>
    public abstract class ContainerLocation : AbstractLocation
    {
        public bool forceShiny;

        public void GetContainer(out GameObject obj, out string containerType)
        {
            (Placement as IContainerPlacement).GetContainer(this, out obj, out containerType);
        }

        public virtual bool Supports(string containerType)
        {
            return containerType == Container.Shiny ? true : !forceShiny;
        }

        public override AbstractPlacement Wrap()
        {
            return new MutablePlacement(name)
            {
                Location = this,
            };
        }
    }
}
