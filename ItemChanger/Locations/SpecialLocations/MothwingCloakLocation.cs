using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// FsmObjectLocation with changes to prevent the dreamer cutscene after the Hornet 1 fight.
    /// </summary>
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

        // TODO: Is there a way to deactivate Dreamer Cutscene 1 without destroying it?
        private void DestroyDreamerScene(Scene to)
        {
            ObjectDestroyer.Destroy(sceneName, "Dreamer Scene 1");
            ObjectDestroyer.Destroy(sceneName, "Hornet Saver");
            ObjectDestroyer.Destroy(sceneName, "Cutscene Dreamer");
            ObjectDestroyer.Destroy(sceneName, "Dream Scene Activate");
        }
    }
}
