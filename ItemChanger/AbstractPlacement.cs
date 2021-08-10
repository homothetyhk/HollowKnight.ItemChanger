using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public abstract class AbstractPlacement
    {
        [Newtonsoft.Json.JsonIgnore]
        public string Name => Location.name;
        [Newtonsoft.Json.JsonIgnore]
        public string SceneName => Location.sceneName;
        public List<AbstractItem> Items { get; set; } = new List<AbstractItem>();
        [Newtonsoft.Json.JsonIgnore]
        public abstract AbstractLocation Location { get; }
        private bool visited;
        IEnumerable<Tag> CombinedTags => Location.tags.Concat(Items.SelectMany(i => i.tags));

        #region Give

        public void GiveAll(MessageType messageType = MessageType.Any, Action callback = null)
        {
            IEnumerator<AbstractItem> enumerator = Items.GetEnumerator();
            GiveInfo info = GetBaseGiveInfo();
            info.Container = MainContainerType;
            info.MessageType = messageType;
            info.Callback = _ => GiveRecursive();
            GiveRecursive();

            void GiveRecursive()
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsObtained())
                    {
                        continue;
                    }

                    enumerator.Current.Give(this, info.Clone());
                    return;
                }

                callback?.Invoke();
            }
        }

        public void GiveAll(GiveInfo info, Action callback = null)
        {
            IEnumerator<AbstractItem> enumerator = Items.GetEnumerator();
            
            GiveRecursive();

            void GiveRecursive()
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsObtained())
                    {
                        continue;
                    }

                    enumerator.Current.Give(this, info.Clone());
                    return;
                }

                callback?.Invoke();
            }
        }

        public void GiveEarly()
        {
            GiveInfo info = GetBaseGiveInfo();
            info.MessageType = MessageType.Corner;
            info.Container = MainContainerType;

            foreach (AbstractItem item in Items)
            {
                if (!item.IsObtained() && item.GiveEarly(MainContainerType)) item.Give(this, info.Clone());
            }
        }

        public string GetUIName()
        {
            return GetUIName(maxLength: 120);
        }

        public string GetUIName(int maxLength)
        {
            IEnumerable<string> itemNames = Items.Where(i => !i.IsObtained()).Select(i => i.UIDef?.GetPostviewName() ?? "Unknown Item");
            string itemText = string.Join(", ", itemNames.ToArray());
            if (itemText.Length > maxLength) itemText = itemText.Substring(0, 117) + "...";
            return itemText;
        }

        private GiveInfo GetBaseGiveInfo()
        {
            return new GiveInfo
            {
                FlingType = Location.flingType,
                Transform = Location.Transform,
            };
        }

        #endregion

        #region Control

        public bool AllObtained()
        {
            return Items.All(i => i.IsObtained());
        }

        public void SetVisited()
        {
            visited = true;
        }

        public bool CheckVisited()
        {
            return visited;
        }

        #endregion

        #region Hooks

        public virtual void OnEnableGlobal(PlayMakerFSM fsm)
        {
            Location.OnEnableGlobal(fsm);
        }

        public virtual void OnEnableLocal(PlayMakerFSM fsm)
        {
            Location.OnEnableLocal(fsm);
        }

        public virtual void OnSceneFetched(Scene target)
        {
            Location.OnSceneFetched(target);
        }

        public virtual void OnActiveSceneChanged(Scene from, Scene to)
        {
            foreach (var tag in Location.GetTags<Tags.IActiveSceneChangedTag>())
            {
                tag.OnActiveSceneChanged(from, to);
            }
            Location.OnActiveSceneChanged(from, to);
        }

        public virtual void OnNextSceneReady(Scene next)
        {
            Location.OnNextSceneReady(next);
        }



        /// <summary>
        /// Override for custom text. Return null if text was not modified.
        /// </summary>
        public virtual string OnLanguageGet(string convo, string sheet)
        {
            return Location.OnLanguageGet(convo, sheet);
        }

        /// <summary>
        /// Called on each location when the location list is first read. Dispose hooks in OnUnHook.
        /// </summary>
        public virtual void OnLoad()
        {
            Location.Placement = this;
            Location.OnLoad();
        }

        public virtual void OnUnload()
        {
            Location.OnUnload();
        }

        #endregion
        [Newtonsoft.Json.JsonIgnore]
        public virtual string MainContainerType => Container.Unknown;
        public virtual string GetContainerType(AbstractItem item) => MainContainerType;
        public virtual string GetContainerType(AbstractLocation location) => MainContainerType;

        public virtual void AddItem(AbstractItem item)
        {
            Items.Add(item);
        }
    }
}
