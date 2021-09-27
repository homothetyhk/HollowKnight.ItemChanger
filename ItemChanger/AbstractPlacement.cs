using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public abstract class AbstractPlacement : TaggableObject
    {
        public AbstractPlacement(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; }
        public List<AbstractItem> Items { get; } = new();
        [JsonProperty]
        public VisitState Visited { get; private set; }

        #region Give

        public void GiveAll(GiveInfo info, Action callback = null)
        {
            IEnumerator<AbstractItem> enumerator = Items.GetEnumerator();
            
            GiveRecursive();

            void GiveRecursive(AbstractItem _ = null)
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

        public string GetUIName()
        {
            return GetUIName(maxLength: 120);
        }

        public string GetUIName(int maxLength)
        {
            IEnumerable<string> itemNames = Items.Where(i => !i.IsObtained()).Select(i => i.UIDef?.GetPreviewName() ?? "Unknown Item");
            string itemText = string.Join(", ", itemNames.ToArray());
            if (itemText.Length > maxLength) itemText = itemText.Substring(0, maxLength > 3 ? maxLength - 3 : 0) + "...";
            return itemText;
        }

        #endregion

        #region Control

        public bool AllObtained()
        {
            return Items.All(i => i.IsObtained());
        }

        public void AddVisitFlag(VisitState flag)
        {
            InvokeVisitStateChanged(flag);
            Visited |= flag;
        }

        public bool CheckVisitedAny(VisitState flags)
        {
            return (Visited & flags) != VisitState.None;
        }

        public bool CheckVisitedAll(VisitState flags)
        {
            return (Visited & flags) == flags;
        }

        public VisitState GetVisitState()
        {
            return Visited;
        }

        #endregion

        #region Hooks

        public void Load()
        {
            LoadTags();
            foreach (AbstractItem item in Items) item.Load();
            OnLoad();
        }

        public void Unload()
        {
            UnloadTags();
            foreach (AbstractItem item in Items) item.Unload();
            OnUnload();
        }


        /// <summary>
        /// Called on each location when the location list is first read. Dispose hooks in OnUnHook.
        /// </summary>
        protected abstract void OnLoad();

        protected abstract void OnUnload();

        public static event Action<VisitStateChangedEventArgs> OnVisitStateChangedGlobal;
        [field: Newtonsoft.Json.JsonIgnore]
        public event Action<VisitStateChangedEventArgs> OnVisitStateChanged;
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
                ItemChangerMod.instance.LogError($"Error invoking OnVisitStateChanged for placement {Name}:\n{e}");
            }
        }

        #endregion

        [Newtonsoft.Json.JsonIgnore]
        public virtual string MainContainerType => Container.Unknown;

        public virtual AbstractPlacement Add(AbstractItem item)
        {
            Items.Add(item);
            return this;
        }

        public AbstractPlacement Add(IEnumerable<AbstractItem> items)
        {
            foreach (var i in items) Add(i);
            return this;
        }

        public AbstractPlacement Add(params AbstractItem[] items)
        {
            foreach (AbstractItem item in items) Add(item);
            return this;
        }
    }
}
