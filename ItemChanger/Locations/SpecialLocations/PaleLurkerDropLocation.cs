using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ItemChanger.Components;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Enemy location which destroys the static simple key shiny that appears upon reentering after killing Pale Lurker.
    /// </summary>
    public class PaleLurkerDropLocation : EnemyLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Shiny Item Key", "Shiny Control"), Destroy);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Shiny Item Key", "Shiny Control"), Destroy);
        }

        private void Destroy(PlayMakerFSM fsm)
        {
            UnityEngine.Object.Destroy(fsm.gameObject);
        }
    }
}