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
    // TODO: Change Dreamer Cutscene 1 into a WorldEvent, instead of destroying it
    public class MothwingCloakLocation : ObjectLocation
    {
        public override void OnEnable(PlayMakerFSM fsm)
        {
            switch (fsm.gameObject.name)
            {
                case "Cloak Corpse" when fsm.FsmName == "Control":
                    {
                        FsmState init = fsm.GetState("Init");
                        init.Actions = new FsmStateAction[] { init.Actions[0], init.Actions[1] }; // remove find child shiny item

                        fsm.GetState("Activate Item").AddAction(new SendEvent
                        {
                            eventTarget = new FsmEventTarget
                            {
                                target = FsmEventTarget.EventTarget.BroadcastAll,
                                excludeSelf = true
                            },
                            sendEvent = FsmEvent.FindEvent("BG OPEN") ?? new FsmEvent("BG OPEN"),
                            delay = 0,
                            everyFrame = false
                        }); // open gate after hornet leaves
                    }
                    break;
                case "Camera Locks Boss" when fsm.FsmName == "FSM":
                    {
                        if (!PlayerData.instance.hornet1Defeated) UnityEngine.Object.Destroy(fsm);
                    }
                    break;
            }
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name != sceneName) return;

            ObjectDestroyer.Destroy(sceneName, "Dreamer Scene 1");
            ObjectDestroyer.Destroy(sceneName, "Hornet Saver");
            ObjectDestroyer.Destroy(sceneName, "Cutscene Dreamer");
            ObjectDestroyer.Destroy(sceneName, "Dream Scene Activate");
        }

        public override void PlaceContainer(GameObject obj, Container containerType)
        {
            base.PlaceContainer(obj, containerType);
            PlayMakerFSM corpseFsm = GameObject.Find("Cloak Corpse").LocateFSM("Control");
            corpseFsm.FsmVariables.FindFsmGameObject("Shiny Item").Value = obj;
        }
    }
}
