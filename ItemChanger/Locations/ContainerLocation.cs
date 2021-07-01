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

        public void GetPrimaryContainer(out GameObject obj, out Container containerType)
        {
            (Placement as IContainerPlacement).GetPrimaryContainer(out obj, out containerType);
        }

        public virtual bool Supports(Container container)
        {
            return container == Container.Shiny ? true : !forceShiny;
        }

        public override AbstractPlacement Wrap()
        {
            return new MutablePlacement
            {
                location = this,
            };
        }
    }
}
