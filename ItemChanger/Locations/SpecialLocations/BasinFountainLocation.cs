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
using ItemChanger.Extensions;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class BasinFountainLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
        }

        private void EditFountain(PlayMakerFSM fsm)
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
