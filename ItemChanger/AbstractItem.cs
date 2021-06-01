using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using UnityEngine;

namespace ItemChanger
{
    public enum Shape
    {
        Shiny,
        Jar,
        Rock
    }


    public abstract class AbstractItem
    {
        public bool obtained;
        public string name;
        public IUIDef UIDef;
        
        public virtual Container GetPreferredContainer() => Container.Shiny;

        public virtual bool GiveEarly(Container container) => false;

        /// <summary>
        /// Method used to determine if a unique item should be replaced (i.e. duplicates, etc). No relation to 'obtained'.
        /// </summary>
        /// <returns></returns>
        public virtual bool Redundant()
        {
            return false;
        }

        public void Give(AbstractPlacement location, Container container, FlingType fling, Transform transform, MessageType message, Action<AbstractItem> callback = null)
        {
            AbstractItem Item = this;
            obtained = true;
            location.visited = true;

            Item.GiveImmediate(container, fling, transform);
            if (Item.UIDef != null)
            {
                Item.UIDef.SendMessage(message, () => callback?.Invoke(Item));
            }
            else callback?.Invoke(Item);
        }

        public abstract void GiveImmediate(Container container, FlingType fling, Transform transform);

        /// <summary>
        /// Determines whether the physical instance of the item has been collected. Should only be set inside Give().
        /// </summary>
        /// <returns></returns>
        public bool IsObtained() => obtained;
    }
}
