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
    /// <summary>
    /// FsmObjectLocation with various changes to support being spawned from the Quake Pickup after Soul Master.
    /// </summary>
    public class DesolateDiveLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Quake Pickup", "Pickup"), EditQuakePickup);
            Events.AddFsmEdit(sceneName, new("BG Control"), EditBGControl);
            Events.AddFsmEdit(sceneName, new("Destroy if Quake"), EditDestroyIfQuake);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Quake Pickup", "Pickup"), EditQuakePickup);
            Events.RemoveFsmEdit(sceneName, new("BG Control"), EditBGControl);
            Events.RemoveFsmEdit(sceneName, new("Destroy if Quake"), EditDestroyIfQuake);
        }

        private void EditQuakePickup(PlayMakerFSM fsm)
        {
            FsmState idle = fsm.GetState("Idle");
            idle.RemoveActionsOfType<IntCompare>();
        }

        private void EditDestroyIfQuake(PlayMakerFSM fsm)
        {
            FsmState check = fsm.GetState("Check");
            check.Actions = new[] { new DelegateBoolTest(() => PlayerData.instance.GetBool(nameof(PlayerData.mageLordDefeated)), "DESTROY", null) };
        }

        private void EditBGControl(PlayMakerFSM fsm)
        {
            foreach (FsmState state in fsm.FsmStates)
            {
                if (state.Transitions.FirstOrDefault(t => t.EventName == "BG OPEN") is FsmTransition transition)
                {
                    state.AddTransition("QUAKE PICKUP START", transition.ToState);
                }
            }
        }
    }
}
