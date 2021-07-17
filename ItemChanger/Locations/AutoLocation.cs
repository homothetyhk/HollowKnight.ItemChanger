using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ItemChanger.Locations
{
    /// <summary>
    /// Location type which cannot accept a container, and thus must implement itself. Examples include items given in dialogue, etc.
    /// </summary>
    public abstract class AutoLocation : AbstractLocation
    {
        public virtual GiveInfo GetGiveInfo()
        {
            return new GiveInfo
            {
                Transform = Transform,
                FlingType = flingType,
                Callback = null,
                Container = Container.Unknown,
                MessageType = MessageType.Any,
            };
        }

        public void GiveAll()
        {
            Placement.GiveAll(GetGiveInfo());
        }

        public void GiveAll(Action callback)
        {
            Placement.GiveAll(GetGiveInfo(), callback);
        }


        public override AbstractPlacement Wrap()
        {
            return new Placements.AutoPlacement
            {
                location = this,
            };
        }

    }
}
