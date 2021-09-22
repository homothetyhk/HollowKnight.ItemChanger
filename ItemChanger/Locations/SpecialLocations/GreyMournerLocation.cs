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
            Events.OnLanguageGet += OnLanguageGet;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Xun NPC", "Conversation Control"), EditXunConvo);
            Events.RemoveFsmEdit(sceneName, new("Heart Piece Folder", "Activate"), EditHeartPieceActivate);
            Events.OnLanguageGet -= OnLanguageGet;
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

        private void OnLanguageGet(LanguageGetArgs args)
        {
            if (HintActive && args.sheet == "Prompts" && args.convo == "XUN_OFFER")
            {
                args.current = $"Accept the Gift, even knowing you'll only get a lousy {Placement.GetUIName()}?";
            }
        }
    }
}
