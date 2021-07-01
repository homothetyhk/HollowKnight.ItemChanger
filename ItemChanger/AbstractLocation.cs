using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger
{
    public abstract class AbstractLocation
    {
        public string name;
        public string sceneName;
        public FlingType flingType;

        [JsonIgnore]
        public Transform Transform { get; set; }

        [JsonIgnore]
        public AbstractPlacement Placement { get; internal set; }

        public virtual void OnSceneFetched(Scene target) { }
        public virtual void OnActiveSceneChanged(Scene from, Scene to) { }
        public virtual void OnNextSceneReady(Scene next) { }
        public virtual void OnEnable(PlayMakerFSM fsm) { }
        public virtual string OnLanguageGet(string convo, string sheet) { return null; }
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
        public abstract AbstractPlacement Wrap();
        public virtual AbstractLocation Clone()
        {
            return (AbstractLocation)MemberwiseClone();
        }
    }
}
