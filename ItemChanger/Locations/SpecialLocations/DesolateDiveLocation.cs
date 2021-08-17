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
    public class DesolateDiveLocation : FsmObjectLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            switch (fsm.FsmName)
            {
                case "Destroy if Quake":
                    {
                        FsmState check = fsm.GetState("Check");
                        check.Actions = new[] { new BoolTestMod(() => PlayerData.instance.GetBool(nameof(PlayerData.mageLordDefeated)), "DESTROY", null) };
                    }
                    break;

                case "BG Control":
                    {
                        foreach (FsmState state in fsm.FsmStates)
                        {
                            if (state.Transitions.FirstOrDefault(t => t.EventName == "BG OPEN") is FsmTransition transition)
                            {
                                state.AddTransition("QUAKE PICKUP START", transition.ToState);
                            }
                        }
                    }
                    break;
                case "Pickup" when fsm.gameObject.name == "Quake Pickup":
                    {
                        FsmState idle = fsm.GetState("Idle");
                        idle.RemoveActionsOfType<IntCompare>();
                    }
                    break;
            }
        }
    }
}
