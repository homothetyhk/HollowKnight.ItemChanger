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
    /// Location for giving items through the Abyss Shriek cutscene.
    /// </summary>
    public class AbyssShriekLocation : AutoLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("Scream 2 Get", "Scream Get"), ChangeShriekGet);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("Scream 2 Get", "Scream Get"), ChangeShriekGet);
        }

        private void ChangeShriekGet(PlayMakerFSM fsm)
        {
            Transform t = fsm.transform;
            if (HintActive) HintBox.Create(t, Placement);

            FsmState init = fsm.GetState("Init");
            init.RemoveActionsOfType<IntCompare>();
            init.AddFirstAction(new DelegateBoolTest(Placement.AllObtained, "INERT", null));

            FsmState uiMsg = fsm.GetState("Ui Msg");
            FsmStateAction give = new AsyncLambda(GiveAllAsync(t), "GET ITEM MSG END");
            uiMsg.Actions = new[] { give };
        }
    }
}
