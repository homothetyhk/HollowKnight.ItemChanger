using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ItemChanger.Components;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    public class PaleLurkerDropLocation : EnemyLocation
    {
        public override void OnEnable(PlayMakerFSM fsm)
        {
            base.OnEnable(fsm);
            if (fsm.gameObject.name == "Shiny Item Key") GameObject.Destroy(fsm.gameObject);
        }
    }
}