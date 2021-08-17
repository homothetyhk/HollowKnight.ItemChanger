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
        public string Container { get; set; }
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
        public GiveEventArgs(AbstractItem orig, AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Orig = orig;
            this.Item = item;
            this.Placement = placement;
            this.Info = info;
        }

        public AbstractItem Orig { get; private set; }
        public AbstractItem Item { get; set; }
        public AbstractPlacement Placement { get; }
        public GiveInfo Info { get; set; }
    }

    public class ReadOnlyGiveEventArgs : EventArgs
    {
        private readonly GiveInfo info;

        public ReadOnlyGiveEventArgs(AbstractItem orig, AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Orig = orig;
            this.Item = item;
            this.Placement = placement;
            this.info = info;
        }

        public AbstractItem Orig { get; private set; }
        public AbstractItem Item { get; private set; }
        public AbstractPlacement Placement { get; private set; }
        public string Container => info.Container;
        public FlingType Fling => info.FlingType;
        public Transform Transform => info.Transform;
        public MessageType MessageType => info.MessageType;
        public Action<AbstractItem> Callback => info.Callback;
    }


    public abstract class AbstractItem : TaggableObject
    {
        [Newtonsoft.Json.JsonProperty]
        private ObtainState obtainState;
        public string name;
        /// <summary>
        /// The UIDef associated to an item. GetResolvedUIDef() should be used instead for most purposes.
        /// </summary>
        public UIDef UIDef;

        public virtual string GetPreferredContainer() => Container.Unknown;

        public virtual bool GiveEarly(string containerType) => false;

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
            ReadOnlyGiveEventArgs readOnlyArgs = new ReadOnlyGiveEventArgs(this, this, placement, info);
            Events.BeforeGiveInvoke(readOnlyArgs);

            GiveEventArgs giveArgs = new GiveEventArgs(this, this, placement, info);
            ResolveItem(giveArgs);

            SetObtained();
            placement.AddVisitFlag(VisitState.ObtainedAnyItem);

            AbstractItem item = giveArgs.Item;
            placement = giveArgs.Placement;
            info = giveArgs.Info;

            readOnlyArgs = new ReadOnlyGiveEventArgs(giveArgs.Orig, giveArgs.Item, giveArgs.Placement, giveArgs.Info);
            Events.OnGiveInvoke(readOnlyArgs);

            item.GiveImmediate(info);
            if (item.UIDef != null)
            {
                item.UIDef.SendMessage(info.MessageType, () => info.Callback?.Invoke(item));
            }
            else info.Callback?.Invoke(item);

            foreach (var t in GetTags<Tags.IGiveEffectTag>())
            {
                t.OnGive(readOnlyArgs);
            }
            Events.AfterGiveInvoke(readOnlyArgs);
        }

        public abstract void GiveImmediate(GiveInfo info);

        public UIDef GetResolvedUIDef(AbstractPlacement placement)
        {
            GiveEventArgs args = new GiveEventArgs(this, this, placement, null);
            ResolveItem(args);
            return args.Item.UIDef;
        }

        public virtual void ResolveItem(GiveEventArgs args)
        {
            foreach (var tag in GetTags<IModifyItemTag>())
            {
                tag.ModifyItem(args);
            }
            Events.ModifyItemInvoke(args);

            if (args.Item?.Redundant() ?? true)
            {
                Events.ModifyRedundantItemInvoke(args);
            }

            if (args.Item == null)
            {
                args.Item = Items.VoidItem.Nothing;
            }
        }


        public void RefreshObtained()
        {
            if (obtainState == ObtainState.Obtained) obtainState = ObtainState.Refreshed;
        }

        public void SetObtained()
        {
            obtainState = ObtainState.Obtained;
        }

        /// <summary>
        /// Determines whether the physical instance of the item has been collected. Should only be set inside Give().
        /// </summary>
        /// <returns></returns>
        public bool IsObtained()
        {
            return obtainState == ObtainState.Obtained;
        }

        public bool WasEverObtained()
        {
            return obtainState != ObtainState.Unobtained;
        }

        public virtual AbstractItem Clone()
        {
            AbstractItem item = (AbstractItem)MemberwiseClone();
            item.UIDef = UIDef?.Clone();
            item.tags = tags?.Where(t => t.Intrinsic)?.ToList();
            return item;
        }
    }
}
