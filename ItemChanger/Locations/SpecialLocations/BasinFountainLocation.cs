using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class BasinFountainLocation : FsmObjectLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == fsmName && fsm.gameObject.name == fsmParent)
            {
                FsmState idle = fsm.GetState("Idle");
                idle.Actions = new FsmStateAction[]
                {
                    idle.Actions[0],
                    idle.Actions[1],
                    // idle.Actions[2], // FindChild -- Vessel Fragment
                    idle.Actions[3],
                };
            }
        }


    }
}
