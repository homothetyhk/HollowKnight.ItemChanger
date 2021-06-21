using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using UnityEngine;

namespace ItemChanger
{
    public class GiveInfo
    {
        public Container Container { get; set; }
        public FlingType FlingType { get; set; }
        public Transform Transform {get; set;}
        public MessageType MessageType { get; set; }
        public Action<AbstractItem> Callback { get; set; }

        public GiveInfo Clone()
        {
            return (GiveInfo)MemberwiseClone();
        }

    }

    public class GiveEventArgs : EventArgs
    {
        public GiveEventArgs(AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Item = item;
            this.Placement = placement;
            this.Info = info;
        }

        public AbstractItem Item { get; set; }
        public AbstractPlacement Placement { get; set; }
        public GiveInfo Info { get; set; }
    }

    public class ReadOnlyGiveEventArgs : EventArgs
    {
        private readonly GiveInfo info;

        public ReadOnlyGiveEventArgs(AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Item = item;
            this.Placement = placement;
            this.info = info;
        }

        public AbstractItem Item { get; private set; }
        public AbstractPlacement Placement { get; private set; }
        public Container Container => info.Container;
        public FlingType Fling => info.FlingType;
        public Transform Transform => info.Transform;
        public MessageType MessageType => info.MessageType;
        public Action<AbstractItem> Callback => info.Callback;
    }


    public abstract class AbstractItem
    {
        private bool obtained;
        public string name;
        public IUIDef UIDef;

        public static event Action<ReadOnlyGiveEventArgs> BeforeGive;
        public static event Action<GiveEventArgs> ModifyGive;
        public static event Action<ReadOnlyGiveEventArgs> OnGive;
        public static event Action<ReadOnlyGiveEventArgs> AfterGive;

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

        public void Give(AbstractPlacement placement, GiveInfo info)
        {
            ReadOnlyGiveEventArgs readOnlyArgs = new ReadOnlyGiveEventArgs(this, placement, info);
            BeforeGive?.Invoke(readOnlyArgs);
            GiveEventArgs modifyArgs = new GiveEventArgs(this, placement, info);
            ModifyGive?.Invoke(modifyArgs);
            readOnlyArgs = new ReadOnlyGiveEventArgs(modifyArgs.Item, modifyArgs.Placement, modifyArgs.Info);

            SetObtained();
            placement.SetVisisted();

            AbstractItem item = modifyArgs.Item;
            placement = modifyArgs.Placement;
            info = modifyArgs.Info;


            item.GiveImmediate(info);
            if (item.UIDef != null)
            {
                item.UIDef.SendMessage(info.MessageType, () => info.Callback?.Invoke(item));
            }
            else info.Callback?.Invoke(item);

            AfterGive?.Invoke(readOnlyArgs);
        }

        public abstract void GiveImmediate(GiveInfo info);

        public void SetObtained()
        {
            obtained = true;
        }

        /// <summary>
        /// Determines whether the physical instance of the item has been collected. Should only be set inside Give().
        /// </summary>
        /// <returns></returns>
        public bool IsObtained() => obtained;
    }
}
