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

            if (HeroController.instance && HeroController.instance.spellControl) FixShriekAnimation(HeroController.instance.spellControl);
        }

        private void FixShriekAnimation(PlayMakerFSM fsm)
        {
            fsm.GetState("Scream Get?").Actions = new FsmStateAction[]
            {
                new BoolTest{ boolVariable = fsm.FsmVariables.FindFsmBool("Scream 2 Zone"), isFalse = FsmEvent.Finished, isTrue = null },
                new Lambda(DoScreamGetBranchUpdate),
                new SendEvent{ eventTarget = FsmEventTarget.Self, sendEvent = FsmEvent.GetFsmEvent("SCREAM GET"), delay = 0f }
            };


            void DoScreamGetBranchUpdate()
            {
                int spellLevel = fsm.FsmVariables.FindFsmInt("Spell Level").Value;

                FsmState screamGetAntic = fsm.GetState("SG Antic");
                FsmState screamGetBurst = fsm.GetState("Scream Burst 3");

                FsmState screamAntic = spellLevel == 1 ? fsm.GetState("Scream Antic1") : fsm.GetState("Scream Antic2");
                FsmState screamBurst = spellLevel == 1 ? fsm.GetState("Scream Burst 1") : fsm.GetState("Scream Burst 2");

                screamGetAntic.GetFirstActionOfType<AudioPlay>().oneShotClip = screamAntic.GetFirstActionOfType<AudioPlay>().oneShotClip;
                screamGetAntic.GetFirstActionOfType<PlayVibration>().highFidelityVibration = screamAntic.GetFirstActionOfType<PlayVibration>().highFidelityVibration;

                screamGetBurst.GetFirstActionOfType<ActivateGameObject>().gameObject = screamBurst.GetActionsOfType<ActivateGameObject>()[1].gameObject;
                screamGetBurst.GetFirstActionOfType<Tk2dPlayAnimationWithEvents>().clipName = screamBurst.GetFirstActionOfType<Tk2dPlayAnimationWithEvents>().clipName;
            }
        }
    }
}
