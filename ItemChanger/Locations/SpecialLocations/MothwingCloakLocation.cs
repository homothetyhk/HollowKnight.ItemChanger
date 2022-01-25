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
            // These objects are in Fungus1_04_boss, but the Fsm hook still catches them in the SuperScene.
            Events.AddFsmEdit(sceneName, new("Dreamer Scene 1", "Control"), DestroyDreamScene); // probably not necessary
            Events.AddFsmEdit(sceneName, new("Cutscene Dreamer", "Control"), DestroyDreamScene); // probably not necessary
            Events.AddFsmEdit(sceneName, new("Dream Scene Activate", "Control"), DestroyDreamScene);
            Events.AddFsmEdit(sceneName, new("Hornet Infected Knight Encounter", "Encounter"), PreventHornetSaver);
            Events.AddFsmEdit(sceneName, new("Hornet Boss 1", "Control"), PreventHornetSaverOnRefight);
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Cloak Corpse", "Control"), EditCloakCorpse);
            Events.RemoveFsmEdit(sceneName, new("Camera Locks Boss", "FSM"), DestroyCameraLock);
            Events.RemoveFsmEdit(sceneName, new("Dreamer Scene 1", "Control"), DestroyDreamScene);
            Events.RemoveFsmEdit(sceneName, new("Cutscene Dreamer", "Control"), DestroyDreamScene);
            Events.RemoveFsmEdit(sceneName, new("Dream Scene Activate", "Control"), DestroyDreamScene);
            Events.RemoveFsmEdit(sceneName, new("Hornet Infected Knight Encounter", "Encounter"), PreventHornetSaver);
            Events.RemoveFsmEdit(sceneName, new("Hornet Boss 1", "Control"), PreventHornetSaverOnRefight);
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
        private void DestroyDreamScene(PlayMakerFSM fsm)
        {
            UObject.Destroy(fsm.gameObject);
        }
        private void PreventHornetSaver(PlayMakerFSM fsm)
        {
            fsm.GetState("Start Fight").RemoveFirstActionOfType<ActivateAllChildren>();
        }

        private void PreventHornetSaverOnRefight(PlayMakerFSM fsm)
        {
            fsm.GetState("Refight Wake").RemoveFirstActionOfType<ActivateAllChildren>();
        }
    }
}
