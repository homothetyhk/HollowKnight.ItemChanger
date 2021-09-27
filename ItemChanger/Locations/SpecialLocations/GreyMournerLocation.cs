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
    public class GreyMournerLocation : FsmObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Xun NPC", "Conversation Control"), EditXunConvo);
            Events.AddFsmEdit(sceneName, new("Heart Piece Folder", "Activate"), EditHeartPieceActivate);
            Events.AddLanguageEdit(new("Prompts", "XUN_OFFER"), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Xun NPC", "Conversation Control"), EditXunConvo);
            Events.RemoveFsmEdit(sceneName, new("Heart Piece Folder", "Activate"), EditHeartPieceActivate);
            Events.RemoveLanguageEdit(new("Prompts", "XUN_OFFER"), OnLanguageGet);
        }

        private void EditXunConvo(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.Actions = init.Actions.Where(a => !(a is FindChild fc) || fc.childName.Value != "Heart Piece").ToArray();

            FsmState crumble = fsm.GetState("Crumble");
            crumble.RemoveActionsOfType<SetFsmGameObject>();
        }

        private void EditHeartPieceActivate(PlayMakerFSM fsm)
        {
            FsmState activate = fsm.GetState("Activate");
            activate.RemoveActionsOfType<FindChild>();
            activate.RemoveActionsOfType<SetFsmBool>();
        }

        private string OnLanguageGet(string orig)
        {
            if (HintActive)
            {
                return $"Accept the Gift, even knowing you'll only get a lousy {Placement.GetUIName()}?";
            }
            else return orig;
        }
    }
}
