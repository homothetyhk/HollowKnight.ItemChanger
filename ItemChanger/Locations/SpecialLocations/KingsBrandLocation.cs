using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which prevents the King's Brand avalanche sequence from occuring.
    /// </summary>
    public class KingsBrandLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Avalanche", "Activate"), Destroy);
            Events.AddFsmEdit(sceneName, new("Avalanche End", "Control"), Destroy);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Avalanche", "Activate"), Destroy);
            Events.RemoveFsmEdit(sceneName, new("Avalanche End", "Control"), Destroy);
        }

        private void Destroy(PlayMakerFSM fsm) => UnityEngine.Object.Destroy(fsm.gameObject);
    }
}
