using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    public class LoreTabletLocation : ObjectLocation
    {
        public string inspectName;
        public string inspectFsm;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);

            // disable inspect region
            if (fsm.FsmName == inspectFsm && fsm.gameObject.name == inspectName)
            {
                if (fsm.GetState("Init") is FsmState init)
                {
                    init.ClearTransitions();
                }

                if (fsm.GetState("Inert") is FsmState inert)
                {
                    inert.ClearTransitions();
                }
            }
        }

    }
}
