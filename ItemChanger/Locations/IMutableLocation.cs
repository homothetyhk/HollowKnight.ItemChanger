using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Location type which supports placing multiple kinds of objects.
    /// </summary>
    public interface IMutableLocation
    {
        string sceneName { get; }
        FlingType flingType { get; }
        bool forceShiny { get; }
        void OnEnable(PlayMakerFSM fsm);
        void OnActiveSceneChanged();
        void Hook();
        void UnHook();
        void PlaceContainer(GameObject obj, Container containerType);
        bool Supports(Container container);
    }
}
