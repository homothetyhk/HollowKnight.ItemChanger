using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ItemChanger.Locations
{
    /// <summary>
    /// Interface required to supply to FsmLocation events. 
    /// </summary>
    public interface IFsmLocationActions
    {
        void Give(Action callback);
        bool AllObtained();
        string GetUIItemName(int maxLength = 120);
        void SetVisited();
        bool CheckVisited();
    }


    /// <summary>
    /// Location type which cannot accept a container. Implemented through OnEnable. Examples include items given in dialogue, etc.
    /// </summary>
    public abstract class FsmLocation
    {
        public string sceneName;
        public FlingType flingType;
        public MessageType messageType;

        /// <summary>
        /// Called during the PlaymakerFSM.OnEnable hook for the given scene.
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="actions"></param>
        /// 
        public abstract void OnEnable(PlayMakerFSM fsm, IFsmLocationActions actions);

        /// <summary>
        /// Called by placement, usually immediately prior to Give.
        /// </summary>
        /// <returns>The (possibly null) transform to use for giving items</returns>
        public virtual Transform FindTransformInScene()
        {
            return null;
        }

        public virtual string OnLanguageGet(string convo, string sheet, Func<string> getItemName)
        {
            return null;
        }
    }
}
