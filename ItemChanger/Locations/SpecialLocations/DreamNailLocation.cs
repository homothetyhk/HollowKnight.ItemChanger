using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Class which modifies the Dream Nail sequence and its triggers. 
    /// Use only when a unique shiny item is placed in Dream_Nailcollection.
    /// </summary>
    public class DreamNailLocation : ObjectLocation
    {
        public override void OnEnable(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Control" && fsm.gameObject.name == "Witch Control")
            {
                fsm.GetState("Convo Ready").RemoveActionsOfType<SetCollider>(); // not important, but prevents null ref unity logs after destroying Moth NPC object
            }

            if (fsm.FsmName == "Shiny Control")
            {
                fsm.FsmVariables.FindFsmBool("Exit Dream").Value = true;
                fsm.GetState("Fade Pause").AddFirstAction(new Lambda(() =>
                {
                    PlayerData.instance.dreamReturnScene = "RestingGrounds_07";
                    Ref.WORLD.dreamNailCutsceneCompleted = true;
                    HeroController.instance.proxyFSM.FsmVariables.GetFsmBool("No Charms").Value = false;
                }));
            }
        }
    }
}
