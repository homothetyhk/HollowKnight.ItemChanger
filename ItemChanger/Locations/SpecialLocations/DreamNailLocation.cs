using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which places a shiny at the end of the Dream Nail sequence that triggers a scene change to the Seer's room. Expects that no other shinies are placed in the Dream Nail sequence.
    /// </summary>
    public class DreamNailLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Witch Control", "Control"), RemoveSetCollider);
            Events.AddFsmEdit(sceneName, new("Shiny Control"), EditShiny);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Witch Control", "Control"), RemoveSetCollider);
            Events.RemoveFsmEdit(sceneName, new("Shiny Control"), EditShiny);
        }

        private void RemoveSetCollider(PlayMakerFSM fsm)
        {
            fsm.GetState("Convo Ready").RemoveActionsOfType<SetCollider>(); // not important, but prevents null ref unity logs after destroying Moth NPC object
        }

        // TODO: implement this to be compatible with Container and to not use WorldEvent
        private void EditShiny(PlayMakerFSM fsm)
        {
            fsm.FsmVariables.FindFsmBool("Exit Dream").Value = true;
            fsm.GetState("Fade Pause").AddFirstAction(new Lambda(() =>
            {
                PlayerData.instance.dreamReturnScene = "RestingGrounds_07";
                HeroController.instance.proxyFSM.FsmVariables.GetFsmBool("No Charms").Value = false;
            }));
        }
    }
}
