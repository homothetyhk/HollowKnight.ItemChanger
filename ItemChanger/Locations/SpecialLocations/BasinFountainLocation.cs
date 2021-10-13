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
    /// FsmObjectLocation with text and fsm edits for spawning an item from the donation fountain.
    /// </summary>
    public class BasinFountainLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
            Events.AddLanguageEdit(new("Prompts", "GEO_RELIEVE"), EditFountainText);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
            Events.RemoveLanguageEdit(new("Prompts", "GEO_RELIEVE"), EditFountainText);
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

        private void EditFountainText(ref string value)
        {
            value = value.Replace("?", $" for {(Placement.Items.Count > 0 ? "a " : "")}{Placement.GetUIName(40)}?");
            Placement.AddVisitFlag(VisitState.Previewed);
        }
    }
}
