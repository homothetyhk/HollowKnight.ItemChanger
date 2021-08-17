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

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            base.PlaceContainer(obj, containerType);
            GameObject xun = ObjectLocation.FindGameObject("Xun NPC");
            if (xun != null)
            {
                xun.LocateFSM("Conversation Control").FsmVariables.FindFsmGameObject("Heart Piece").Value = obj;
            }
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.FsmName)
            {
                case "Conversation Control" when fsm.gameObject.name == "Xun NPC":
                    {
                        FsmState init = fsm.GetState("Init");
                        init.Actions = init.Actions.Where(a => !(a is FindChild fc) || fc.childName.Value != "Heart Piece").ToArray();

                        FsmState crumble = fsm.GetState("Crumble");
                        crumble.RemoveActionsOfType<SetFsmGameObject>();
                    }
                    break;
                case "Activate" when fsm.gameObject.name == "Heart Piece Folder":
                    {
                        FsmState activate = fsm.GetState("Activate");
                        activate.RemoveActionsOfType<FindChild>();
                        activate.RemoveActionsOfType<SetFsmBool>();
                    }
                    break;
            }
        }

        public override void OnLanguageGet(LanguageGetArgs args)
        {
            if (HintActive && args.sheet == "Prompts" && args.convo == "XUN_OFFER")
            {
                args.current = $"Accept the Gift, even knowing you'll only get a lousy {Placement.GetUIName()}?";
            }
        }
    }
}
