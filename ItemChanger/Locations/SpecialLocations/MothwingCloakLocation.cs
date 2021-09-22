using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    // TODO: Change Dreamer Cutscene 1 into a WorldEvent, instead of destroying it
    public class MothwingCloakLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Cloak Corpse", "Control"), EditCloakCorpse);
            Events.AddFsmEdit(sceneName, new("Camera Locks Boss", "FSM"), DestroyCameraLock);
            Events.AddSceneChangeEdit(sceneName, DestroyDreamerScene);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Cloak Corpse", "Control"), EditCloakCorpse);
            Events.RemoveFsmEdit(sceneName, new("Camera Locks Boss", "FSM"), DestroyCameraLock);
            Events.RemoveSceneChangeEdit(sceneName, DestroyDreamerScene);
        }

        private void EditCloakCorpse(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.Actions = new FsmStateAction[] { init.Actions[0], init.Actions[1] }; // remove find child shiny item

            fsm.GetState("Activate Item").AddLastAction(new SendEvent
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

        private void DestroyCameraLock(PlayMakerFSM fsm)
        {
            if (!PlayerData.instance.hornet1Defeated) UnityEngine.Object.Destroy(fsm);
        }

        private void DestroyDreamerScene(Scene to)
        {
            ObjectDestroyer.Destroy(sceneName, "Dreamer Scene 1");
            ObjectDestroyer.Destroy(sceneName, "Hornet Saver");
            ObjectDestroyer.Destroy(sceneName, "Cutscene Dreamer");
            ObjectDestroyer.Destroy(sceneName, "Dream Scene Activate");
        }
    }
}
