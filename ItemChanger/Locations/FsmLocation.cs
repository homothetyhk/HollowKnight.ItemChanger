using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ItemChanger.Locations
{
    /// <summary>
    /// Location type which cannot accept a container. Implemented through OnEnable. Examples include items given in dialogue, etc.
    /// </summary>
    public abstract class FsmLocation : AbstractLocation
    {
        public abstract MessageType MessageType { get; }

        /// <summary>
        /// Called by placement, usually immediately prior to Give.
        /// </summary>
        /// <returns>The (possibly null) transform to use for giving items</returns>
        public virtual Transform FindTransformInScene()
        {
            return null;
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.FsmPlacement
            {
                location = this,
            };
        }

    }
}
