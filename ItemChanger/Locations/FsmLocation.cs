using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    public abstract class FsmLocation
    {
        public string sceneName;
        public FlingType flingType;
        public MessageType messageType;

        public abstract void OnEnable(PlayMakerFSM fsm, Func<bool> boolTest, Action<Action> giveAction);

        /// <summary>
        /// Called by placement, usually immediately prior to Give.
        /// </summary>
        /// <returns>The (possibly null) transform to use for giving items</returns>
        public virtual Transform FindTransformInScene()
        {
            return null;
        }

        public virtual bool CallLanguageHook(string convo, string sheet)
        {
            return false;
        }

        public virtual string OnLanguageGet(string convo, string sheet, string item)
        {
            return null;
        }
    }
}
