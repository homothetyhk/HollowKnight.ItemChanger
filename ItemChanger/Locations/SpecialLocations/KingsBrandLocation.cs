﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations.SpecialLocations
{
    public class KingsBrandLocation : ObjectLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.gameObject.name)
            {
                case "Avalanche" when fsm.FsmName == "Activate":
                case "Avalanche End" when fsm.FsmName == "Control":
                    UnityEngine.Object.Destroy(fsm.gameObject);
                    break;
            }
        }
    }
}
