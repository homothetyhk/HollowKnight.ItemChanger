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
        public Container Container => info.Container;
        public FlingType Fling => info.FlingType;
        public Transform Transform => info.Transform;
        public MessageType MessageType => info.MessageType;
        public Action<AbstractItem> Callback => info.Callback;
    }


    public abstract class AbstractItem
    {
        [Newtonsoft.Json.JsonProperty]
        private ObtainState obtainState;
        public string name;
        public IUIDef UIDef;
        public Persistence persistence;
        public List<Tag> tags = new List<Tag>();

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
            ReadOnlyGiveEventArgs readOnlyArgs = new ReadOnlyGiveEventArgs(this, this, placement, info);
            Events.BeforeGiveInvoke(readOnlyArgs);

            GiveEventArgs giveArgs = new GiveEventArgs(this, this, placement, info);
            ResolveItem(giveArgs);

            SetObtained();
            placement.SetVisited();

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

            Events.AfterGiveInvoke(readOnlyArgs);
        }

        public abstract void GiveImmediate(GiveInfo info);

        public IUIDef GetResolvedUIDef(AbstractPlacement placement)
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

        public T GetTag<T>()
        {
            return tags.OfType<T>().FirstOrDefault();
        }

        public bool GetTag<T>(out T t)
        {
            t = tags.OfType<T>().FirstOrDefault();
            return t != null;
        }

        public T GetOrAddTag<T>() where T : Tag, new()
        {
            return tags.OfType<T>().FirstOrDefault() ?? AddTag<T>();
        }

        public IEnumerable<T> GetTags<T>()
        {
            return tags.OfType<T>();
        }

        public T AddTag<T>() where T : Tag, new()
        {
            T t = new T();
            tags.Add(t);
            return t;
        }

        public bool HasTag<T>() where T : Tag
        {
            return tags.OfType<T>().Any();
        }

        public virtual AbstractItem Clone()
        {
            AbstractItem item = (AbstractItem)MemberwiseClone();
            item.tags = tags.Where(t => t.Intrinsic).ToList();
            return item;
        }
    }
}
