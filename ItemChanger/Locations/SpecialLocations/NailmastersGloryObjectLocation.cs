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
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Class which modifies the Sly Basement sequence and triggers.
    /// Use only when a unique shiny item is placed in Room_Sly_Storeroom.
    /// </summary>
    public class NailmastersGloryObjectLocation : ObjectLocation
    {
        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Shiny Control")
            {
                fsm.FsmVariables.FindFsmBool("Exit Dream").Value = true;
                fsm.GetState("Fade Pause").AddFirstAction(new Lambda(() =>
                {
                    PlayerData.instance.dreamReturnScene = "Town";
                    Internal.Ref.WORLD.slyBasementCompleted = true;
                }));
            }
        }
    }
}
