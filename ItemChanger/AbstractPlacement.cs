using Newtonsoft.Json;
using System.Text;

namespace ItemChanger
{
    /// <summary>
    /// The base class for all placements. Placements carry a list of items and specify how to implement those items, often using locations.
    /// </summary>
    public abstract class AbstractPlacement : TaggableObject
    {
        /// <summary>
        /// Creates a placement with the given name.
        /// </summary>
        public AbstractPlacement(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// The name of the placement. Placement names are enforced to be unique.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The items attached to the placement.
        /// </summary>
        public List<AbstractItem> Items { get; } = new();

        /// <summary>
        /// An enumeration of visit flags accrued by the placement. Which flags may be set depends on the placement type and other factors.
        /// </summary>
        [JsonProperty]
        public VisitState Visited { get; private set; }

        #region Give

        /// <summary>
        /// Helper method for giving all of the items of the placement in sequence, so that the UIDef message of one leads into giving the next.
        /// </summary>
        public void GiveAll(GiveInfo info, Action? callback = null)
        {
            IEnumerator<AbstractItem> enumerator = Items.GetEnumerator();
            
            GiveRecursive();

            void GiveRecursive(AbstractItem? _ = null)
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsObtained())
                    {
                        continue;
                    }

                    var next = info.Clone();
                    next.Callback = GiveRecursive;
                    enumerator.Current.Give(this, next);
                    return;
                }

                callback?.Invoke();
            }
        }

        public virtual void OnObtainedItem(AbstractItem item)
        {
            AddVisitFlag(VisitState.ObtainedAnyItem);
        }

        public virtual void OnPreview(string previewText)
        {
            GetOrAddTag<Tags.PreviewRecordTag>().previewText = previewText;
            AddVisitFlag(VisitState.Previewed);
        }

        /// <summary>
        /// Combines and returns the preview names of the unobtained items at the placement. Used for most hints or previews.
        /// </summary>
        public string GetUIName()
        {
            return GetUIName(maxLength: 120);
        }

        /// <summary>
        /// Combines and returns the preview names of the unobtained items at the placement, trimmed to the specified length.
        /// </summary>
        public string GetUIName(int maxLength)
        {
            IEnumerable<string> itemNames = Items.Where(i => !i.IsObtained()).Select(i => i.GetPreviewName(this) ?? Language.Language.Get("UNKNOWN_ITEM", "IC"));
            string itemText = string.Join(Language.Language.Get("COMMA_SPACE", "IC"), itemNames.ToArray());
            if (itemText.Length > maxLength) itemText = itemText.Substring(0, maxLength > 3 ? maxLength - 3 : 0) + "...";
            return itemText;
        }

        #endregion

        #region Control

        /// <summary>
        /// Returns true when the placement currently has no items to give.
        /// </summary>
        public bool AllObtained()
        {
            return Items.All(i => i.IsObtained());
        }

        /// <summary>
        /// Sets the visit state of the placement to the union of its current flags and the parameter flags.
        /// </summary>
        public void AddVisitFlag(VisitState flag)
        {
            InvokeVisitStateChanged(flag);
            Visited |= flag;
        }

        /// <summary>
        /// Returns true if the flags have nonempty intersection with the placement's visit state.
        /// </summary>
        public bool CheckVisitedAny(VisitState flags)
        {
            return (Visited & flags) != VisitState.None;
        }

        /// <summary>
        /// Returns true if the flags are a subset of the placement's visit state.
        /// </summary>
        public bool CheckVisitedAll(VisitState flags)
        {
            return (Visited & flags) == flags;
        }

        #endregion

        #region Hooks

        /// <summary>
        /// Called on each saved placement when the save is created or resumed.
        /// <br/>Execution order is (modules load -> placement tags load -> items load -> placements load)
        /// </summary>
        public void Load()
        {
            LoadTags();
            foreach (AbstractItem item in Items) item.Load();
            OnLoad();
        }

        /// <summary>
        /// Called on each saved placement upon returning to main menu.
        /// <br/>Execution order is (modules unload -> placement tags unload -> items unload -> placements unload)
        /// </summary>
        public void Unload()
        {
            UnloadTags();
            foreach (AbstractItem item in Items) item.Unload();
            OnUnload();
        }


        /// <summary>
        /// Called by Load(). Dispose hooks in OnUnload.
        /// </summary>
        protected abstract void OnLoad();

        /// <summary>
        /// Called by Unload().
        /// </summary>
        protected abstract void OnUnload();

        /// <summary>
        /// Event invoked by each placement whenever new flags are added to its Visited. Skipped if added flags are a subset of Visited.
        /// </summary>
        public static event Action<VisitStateChangedEventArgs>? OnVisitStateChangedGlobal;

        /// <summary>
        /// Event invoked by this placement whenever AddVisitFlag is called. Use the NoChange property of the args to detect whether a change will occur.
        /// </summary>
        [field: JsonIgnore]
        public event Action<VisitStateChangedEventArgs>? OnVisitStateChanged;
        private void InvokeVisitStateChanged(VisitState newFlags)
        {
            VisitStateChangedEventArgs args = new(this, newFlags);
            try
            {
                OnVisitStateChangedGlobal?.Invoke(args);
                OnVisitStateChanged?.Invoke(args);
            }
            catch (Exception e)
            {
                LogError($"Error invoking OnVisitStateChanged for placement {Name}:\n{e}");
            }
        }

        #endregion

        /// <summary>
        /// The container type that best describes the placement as a whole.
        /// </summary>
        [JsonIgnore]
        public virtual string MainContainerType => Container.Unknown;

        public virtual IEnumerable<Tag> GetPlacementAndLocationTags()
        {
            return tags ?? Enumerable.Empty<Tag>();
        }


        /// <summary>
        /// Adds an item to the item list.
        /// </summary>
        public virtual AbstractPlacement Add(AbstractItem item)
        {
            Items.Add(item);
            return this;
        }

        /// <summary>
        /// Adds a range of items to the item list.
        /// </summary>
        public AbstractPlacement Add(IEnumerable<AbstractItem> items)
        {
            foreach (var i in items) Add(i);
            return this;
        }

        /// <summary>
        /// Adds a range of items to the item list.
        /// </summary>
        public AbstractPlacement Add(params AbstractItem[] items)
        {
            foreach (AbstractItem item in items) Add(item);
            return this;
        }
    }
}
