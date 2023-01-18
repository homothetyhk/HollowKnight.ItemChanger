using Newtonsoft.Json;

namespace ItemChanger
{
    /// <summary>
    /// The parameters included when an item is given. May be null.
    /// </summary>
    public class GiveInfo
    {
        /// <summary>
        /// The best description of the most specific container for this item.
        /// </summary>
        public string? Container { get; set; }
        /// <summary>
        /// How geo and similar objects are allowed to be flung.
        /// </summary>
        public FlingType FlingType { get; set; }
        /// <summary>
        /// The transform to use for flinging and similar actions. May be null.
        /// </summary>
        public Transform? Transform {get; set;}
        /// <summary>
        /// A flag enumeration of the allowed message types for the UIDef after the item is given.
        /// </summary>
        public MessageType MessageType { get; set; }
        /// <summary>
        /// A callback set by the location or placement to be executed by the UIDef when its message is complete.
        /// </summary>
        public Action<AbstractItem>? Callback { get; set; }

        /// <summary>
        /// Returns a shallow clone of the GiveInfo.
        /// </summary>
        public GiveInfo Clone()
        {
            return (GiveInfo)MemberwiseClone();
        }
    }

    /// <summary>
    /// The base class for all items.
    /// </summary>
    public abstract class AbstractItem : TaggableObject
    {
        [JsonProperty]
        private ObtainState obtainState;
        
        /// <summary>
        /// The name of the item. Item names are not guaranteed to be unique.
        /// </summary>
        public string name;

        /// <summary>
        /// The UIDef associated to an item. GetResolvedUIDef() is preferred in most cases, since it accounts for the hooks which may modify the item.
        /// </summary>
        public UIDef? UIDef;

        /// <summary>
        /// Method allowing derived item classes to initialize and place hooks.
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// Called on each item tied to a placement when the save is created or resumed.
        /// <br/>Execution order is (modules load -> placement tags load -> items load -> placements load)
        /// </summary>
        public void Load()
        {
            LoadTags();
            OnLoad();
        }

        /// <summary>
        /// Method allowing derived item classes to dispose hooks.
        /// </summary>
        protected virtual void OnUnload() { }

        /// <summary>
        /// Called on each item tied to a placement upon returning to the main menu.
        /// <br/>Execution order is (modules unload -> placement tags unload -> items unload -> placements unload)
        /// </summary>
        public void Unload()
        {
            UnloadTags();
            OnUnload();
        }

        /// <summary>
        /// Used by some placements to decide what container to use for the item. A value of "Unknown" is ignored, and usually leads to a shiny item by default.
        /// </summary>
        public virtual string GetPreferredContainer() => Container.Unknown;

        /// <summary>
        /// Indicates that the item can be given early in a special way from the given container.
        /// <br/> For example, SpawnGeoItem can be given early from Container.Chest by flinging geo directly from the chest.
        /// </summary>
        public virtual bool GiveEarly(string containerType) => false;

        /// <summary>
        /// Method used to determine if a unique item should be replaced (i.e. duplicates, etc). No relation to ObtainState.
        /// </summary>
        /// <returns></returns>
        public virtual bool Redundant()
        {
            return false;
        }

        /// <summary>
        /// The method called to give an item.
        /// </summary>
        public void Give(AbstractPlacement? placement, GiveInfo info)
        {
            ObtainState originalState = obtainState;
            ReadOnlyGiveEventArgs readOnlyArgs = new(this, this, placement, info, originalState);
            BeforeGiveInvoke(readOnlyArgs);

            GiveEventArgs giveArgs = new(this, this, placement, info, originalState);
            ResolveItem(giveArgs);

            SetObtained();
            placement?.OnObtainedItem(this);

            AbstractItem item = giveArgs.Item;
            info = giveArgs.Info!;

            readOnlyArgs = new(giveArgs.Orig, item, placement, info, originalState);
            OnGiveInvoke(readOnlyArgs);

            try
            {
                item.GiveImmediate(info);
            }
            catch (Exception e)
            {
                LogError($"Error on GiveImmediate for item {item.name}:\n{e}");
                Internal.MessageController.Error();
            }

            AfterGiveInvoke(readOnlyArgs);

            if (item.UIDef != null)
            {
                try
                {
                    item.UIDef.SendMessage(info.MessageType, () => info.Callback?.Invoke(item));
                }
                catch (Exception e)
                {
                    LogError($"Error on SendMessage for item {item.name}:\n{e}");
                    Internal.MessageController.Error();
                    info.Callback?.Invoke(item);
                }
            }
            else info.Callback?.Invoke(item);
        }

        /// <summary>
        /// Specifies the effect of giving a particular item.
        /// </summary>
        public abstract void GiveImmediate(GiveInfo info);

        public string GetPreviewName(AbstractPlacement? placement = null)
        {
            if (HasTag<Tags.DisableItemPreviewTag>() 
                || (placement != null && placement.HasTag<Tags.DisableItemPreviewTag>())) return Language.Language.Get("???", "IC");
            UIDef? def = GetResolvedUIDef(placement);
            return def?.GetPreviewName() ?? Language.Language.Get("???", "IC"); ;
        }

        public Sprite? GetPreviewSprite(AbstractPlacement? placement = null)
        {
            if (HasTag<Tags.DisableItemPreviewTag>()
                || (placement != null && placement.HasTag<Tags.DisableItemPreviewTag>())) return Modding.CanvasUtil.NullSprite();
            UIDef? def = GetResolvedUIDef(placement);
            return def?.GetSprite();
        }

        /// <summary>
        /// Returns the UIDef of the item yielded after all of the events for modifying items.
        /// </summary>
        public UIDef? GetResolvedUIDef(AbstractPlacement? placement = null)
        {
            GiveEventArgs args = new(this, this, placement, null, obtainState);
            ResolveItem(args);
            return args.Item.UIDef;
        }

        /// <summary>
        /// Determines the item yielded after all of the events for modifying items, by acting in place on the GiveEventArgs.
        /// </summary>
        public virtual void ResolveItem(GiveEventArgs args)
        {
            ModifyItemInvoke(args);

            if (args.Item?.Redundant() ?? true)
            {
                ModifyRedundantItemInvoke(args);
            }

            args.Item ??= Items.VoidItem.Nothing;
        }

        /// <summary>
        /// Marks the item as available to be given again. Used, for example, with persistent and semipersistent items.
        /// </summary>
        public void RefreshObtained()
        {
            if (obtainState == ObtainState.Obtained) obtainState = ObtainState.Refreshed;
        }

        /// <summary>
        /// Marks the item as obtained and no longer eligible to be given. Called by Give().
        /// </summary>
        public void SetObtained()
        {
            obtainState = ObtainState.Obtained;
        }

        /// <summary>
        /// Returns whether the item is currently obtained. A value of true indicates the item is not eligible to be given.
        /// </summary>
        /// <returns></returns>
        public bool IsObtained()
        {
            return obtainState == ObtainState.Obtained;
        }

        /// <summary>
        /// Returns whether the item has ever been obtained, regardless of whether it is currently refreshed.
        /// </summary>
        public bool WasEverObtained()
        {
            return obtainState != ObtainState.Unobtained;
        }

        /// <summary>
        /// Returns a deep clone of the current item.
        /// </summary>
        public virtual AbstractItem Clone()
        {
            AbstractItem item = (AbstractItem)MemberwiseClone();
            item.UIDef = UIDef?.Clone();
            item.tags = tags?.Select(t => t.Clone())?.ToList();
            return item;
        }

        /// <summary>
        /// Event invoked by this item at the start of Give(), giving access to the initial give parameters.
        /// </summary>
        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs>? BeforeGive;

        /// <summary>
        /// Event invoked by each item at the start of Give(), giving access to the initial give parameters.
        /// </summary>
        public static event Action<ReadOnlyGiveEventArgs>? BeforeGiveGlobal;
        
        private void BeforeGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                BeforeGiveGlobal?.Invoke(args);
                BeforeGive?.Invoke(args);
            }
            catch (Exception e)
            {
                string? placement = args?.Placement?.Name;
                if (placement != null)
                {
                    LogError($"Error invoking BeforeGive for item {name} at placement {placement}:\n{e}");
                }
                else
                {
                    LogError($"Error invoking BeforeGive for item {name} with placement unavailable:\n{e}");
                }
            }
        }

        /// <summary>
        /// Event invoked by this item during Give() to allow modification of any of the give parameters, including the item given.
        /// </summary>
        [field: JsonIgnore]
        public event Action<GiveEventArgs>? ModifyItem;

        /// <summary>
        /// Event invoked by each item during Give() to allow modification of any of the give parameters, including the item given.
        /// </summary>
        public static event Action<GiveEventArgs>? ModifyItemGlobal;

        private void ModifyItemInvoke(GiveEventArgs args)
        {
            try
            {
                ModifyItemGlobal?.Invoke(args);
                ModifyItem?.Invoke(args);
            }
            catch (Exception e)
            {
                string? placement = args?.Placement?.Name;
                if (placement != null)
                {
                    LogError($"Error invoking ModifyItem for item {name} at placement {placement}:\n{e}");
                }
                else
                {
                    LogError($"Error invoking ModifyItem for item {name} with placement unavailable:\n{e}");
                }
            }
        }

        /// <summary>
        /// Event invoked by this item after the ModifyItem events, if the resulting item is null or redundant.
        /// </summary>
        [field: JsonIgnore]
        public event Action<GiveEventArgs>? ModifyRedundantItem;

        /// <summary>
        /// Event invoked by each item after the ModifyItem events, if the resulting item is null or redundant.
        /// </summary>
        public static event Action<GiveEventArgs>? ModifyRedundantItemGlobal;
        
        private void ModifyRedundantItemInvoke(GiveEventArgs args)
        {
            try
            {
                ModifyRedundantItemGlobal?.Invoke(args);
                ModifyRedundantItem?.Invoke(args);
            }
            catch (Exception e) 
            {
                string? placement = args?.Placement?.Name;
                if (placement != null)
                {
                    LogError($"Error invoking ModifyRedundantItem for item {name} at placement {placement}:\n{e}");
                }
                else
                {
                    LogError($"Error invoking ModifyRedundantItem for item {name} with placement unavailable:\n{e}");
                }
            }
        }

        /// <summary>
        /// Event invoked by this item just before GiveImmediate(), giving access to the final give parameters.
        /// </summary>
        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs>? OnGive;

        /// <summary>
        /// Event invoked by each item just before GiveImmediate(), giving access to the final give parameters.
        /// </summary>
        public static event Action<ReadOnlyGiveEventArgs>? OnGiveGlobal;
        
        private void OnGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                OnGiveGlobal?.Invoke(args);
                OnGive?.Invoke(args);
            }
            catch (Exception e) 
            {
                string? placement = args?.Placement?.Name;
                if (placement != null)
                {
                    LogError($"Error invoking OnGive for item {name} at placement {placement}:\n{e}");
                }
                else
                {
                    LogError($"Error invoking OnGive for item {name} with placement unavailable:\n{e}");
                }
            }
        }

        /// <summary>
        /// Event invoked by this item just after GiveImmediate(), giving access to the final give parameters.
        /// </summary>
        [field: JsonIgnore]
        public event Action<ReadOnlyGiveEventArgs>? AfterGive;

        /// <summary>
        /// Event invoked by each item just after GiveImmediate(), giving access to the final give parameters.
        /// </summary>
        public static event Action<ReadOnlyGiveEventArgs>? AfterGiveGlobal;
        
        private void AfterGiveInvoke(ReadOnlyGiveEventArgs args)
        {
            try
            {
                AfterGiveGlobal?.Invoke(args);
                AfterGive?.Invoke(args);
            }
            catch (Exception e) 
            {
                string? placement = args?.Placement?.Name;
                if (placement != null)
                {
                    LogError($"Error invoking AfterGive for item {name} at placement {placement}:\n{e}");
                }
                else
                {
                    LogError($"Error invoking AfterGive for item {name} with placement unavailable:\n{e}");
                }
            }
        }
    }
}
