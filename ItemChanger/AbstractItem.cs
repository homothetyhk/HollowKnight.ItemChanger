using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using Newtonsoft.Json;
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


    public abstract class AbstractItem : TaggableObject
    {
        [Newtonsoft.Json.JsonProperty]
        private ObtainState obtainState;
        public string name;
        /// <summary>
        /// The UIDef associated to an item. GetResolvedUIDef() should be used instead for most purposes.
        /// </summary>
        public UIDef UIDef;

        protected virtual void OnLoad() { }
        public void Load()
        {
            LoadTags();
            OnLoad();
        }

        protected virtual void OnUnload() { }
        public void Unload()
        {
            UnloadTags();
            OnUnload();
        }



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
            BeforeGiveInvoke(readOnlyArgs);

            GiveEventArgs giveArgs = new GiveEventArgs(this, this, placement, info);
            ResolveItem(giveArgs);

            SetObtained();
            placement.AddVisitFlag(VisitState.ObtainedAnyItem);

            AbstractItem item = giveArgs.Item;
            placement = giveArgs.Placement;
            info = giveArgs.Info;

            readOnlyArgs = new ReadOnlyGiveEventArgs(giveArgs.Orig, giveArgs.Item, giveArgs.Placement, giveArgs.Info);
            OnGiveInvoke(readOnlyArgs);

            try
            {
                item.GiveImmediate(info);
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError($"Error on GiveImmediate for item {item?.name}:\n{e}");
                Internal.MessageController.Error();
            }
            
            if (item.UIDef != null)
            {
                try
                {
                    item.UIDef.SendMessage(info.MessageType, () => info.Callback?.Invoke(item));
                }
                catch (Exception e)
                {
                    ItemChangerMod.instance.LogError($"Error on SendMessage for item {item?.name}:\n{e}");
                    Internal.MessageController.Error();
                    info.Callback?.Invoke(item);
                }
            }
            else info.Callback?.Invoke(item);

            AfterGiveInvoke(readOnlyArgs);
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
            ModifyItemInvoke(args);

            if (args.Item?.Redundant() ?? true)
            {
                ModifyRedundantItemInvoke(args);
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
            item.tags = tags?.Select(t => t.Clone())?.ToList();
            return item;
        }

        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs> BeforeGive;
        public static event Action<ReadOnlyGiveEventArgs> BeforeGiveGlobal;
        private void BeforeGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                BeforeGiveGlobal?.Invoke(args);
                BeforeGive?.Invoke(args);
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError($"Error invoking BeforeGive for item {name} at placement {args.Placement.Name}:\n{e}");
            }
        }

        [field: JsonIgnore]
        public event Action<GiveEventArgs> ModifyItem;
        public static event Action<GiveEventArgs> ModifyItemGlobal;
        private void ModifyItemInvoke(GiveEventArgs args)
        {
            try
            {
                ModifyItemGlobal?.Invoke(args);
                ModifyItem?.Invoke(args);
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError($"Error invoking ModifyItem for item {name} at placement {args.Placement.Name}:\n{e}");
            }
        }

        [field: JsonIgnore]
        public event Action<GiveEventArgs> ModifyRedundantItem;
        public static event Action<GiveEventArgs> ModifyRedundantItemGlobal;
        private void ModifyRedundantItemInvoke(GiveEventArgs args)
        {
            try
            {
                ModifyRedundantItemGlobal?.Invoke(args);
                ModifyRedundantItem?.Invoke(args);
            }
            catch (Exception e) 
            {
                ItemChangerMod.instance.LogError($"Error invoking ModifyRedundantItem for item {name} at placement {args.Placement.Name}:\n{e}"); 
            }
        }

        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs> OnGive;
        public static event Action<ReadOnlyGiveEventArgs> OnGiveGlobal;
        private void OnGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                OnGiveGlobal?.Invoke(args);
                OnGive?.Invoke(args);
            }
            catch (Exception e) 
            {
                ItemChangerMod.instance.LogError($"Error invoking OnGive for item {name} at placement {args.Placement.Name}:\n{e}"); 
            }
        }

        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs> AfterGive;
        public static event Action<ReadOnlyGiveEventArgs> AfterGiveGlobal;
        private void AfterGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                AfterGiveGlobal?.Invoke(args);
                AfterGive?.Invoke(args);
            }
            catch (Exception e) 
            {
                ItemChangerMod.instance.LogError($"Error invoking BeforeGive for item {name} at placement {args.Placement.Name}:\n{e}"); 
            }
        }

    }
}
