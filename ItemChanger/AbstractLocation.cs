using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public abstract class AbstractLocation : TaggableObject
    {
        public string name;
        public string sceneName;
        public FlingType flingType;

        [JsonIgnore]
        public AbstractPlacement Placement { get; internal set; }

        public void Load()
        {
            LoadTags();
            OnLoad();
        }

        public void Unload()
        {
            UnloadTags();
            OnUnload();
        }

        protected abstract void OnLoad();
        protected abstract void OnUnload();

        public abstract AbstractPlacement Wrap();
        public virtual AbstractLocation Clone()
        {
            AbstractLocation location = (AbstractLocation)MemberwiseClone();
            location.tags = location.tags?.Select(t => t.Clone())?.ToList();
            return location;
        }
    }
}
