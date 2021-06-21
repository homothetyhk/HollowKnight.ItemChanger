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
    public class NailmasterLocation : FsmLocation
    {
        public string objectName;
        public string fsmName;

        public override void OnEnable(PlayMakerFSM fsm, IFsmLocationActions actions)
        {
            if (fsm.FsmName == fsmName && fsm.gameObject.name == objectName)
            {
                FsmState convo = fsm.GetState("Convo Choice");
                FsmState getMsg = fsm.GetState("Get Msg");
                FsmState fade = fsm.GetState("Fade Back");

                FsmStateAction test = new BoolTestMod(actions.AllObtained, null, "REOFFER");
                void Callback()
                {
                    fsm.SendEvent("GET ITEM MSG END");
                }

                FsmStateAction give = new Lambda(() => actions.Give(Callback));

                convo.Actions[objectName == "NM Sheo NPC" ? 2 : 1] = test;

                getMsg.Actions = new FsmStateAction[]
                {
                    getMsg.Actions[0],
                    getMsg.Actions[1],
                    getMsg.Actions[2],
                    give,
                };
            }
        }

        public override string OnLanguageGet(string convo, string sheet, Func<string> getItemName)
        {
            if (sheet == "Prompts" && convo == "NAILMASTER_FREE") return getItemName();
            return null;
        }

        public override Transform FindTransformInScene()
        {
            return ObjectLocation.FindGameObject(objectName)?.transform;
        }
    }
}
