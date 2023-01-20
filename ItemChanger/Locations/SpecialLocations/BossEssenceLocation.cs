using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location for giving an item from talking to the defeated boss ghost.
    /// </summary>
    public class BossEssenceLocation : AutoLocation
    {
        public string fsmName;
        public string? objName;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objName, fsmName), EditBossConvo);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objName, fsmName), EditBossConvo);
        }

        private void EditBossConvo(PlayMakerFSM fsm)
        {
            if (fsmName == "Award Orbs")
            {
                // Add give
                FsmState get = fsm.GetState("Award");
                List<FsmStateAction> fsmActions = get.Actions.ToList();
                fsmActions.RemoveAt(fsmActions.Count - 1); // SendEventByName (essence counter)
                fsmActions.RemoveAt(fsmActions.Count - 1); // PlayerDataIntAdd (add essence)
                fsmActions.Add(new AsyncLambda(GiveAllAsync(fsm.transform)));
                get.SetActions(fsmActions.ToArray());

                bool Test()
                {
                    return !HeroController.instance.controlReqlinquished 
                        && HeroController.instance.transitionState == GlobalEnums.HeroTransitionState.WAITING_TO_TRANSITION;
                }

                // Fsm change to allow for re-give
                if (Placement.CheckVisitedAny(VisitState.ObtainedAnyItem) && !Placement.AllObtained())
                {
                    FsmState idle = fsm.GetState("Idle");
                    idle.AddLastAction(new WaitForDelegate(Test, "DREAM WAKE"));
                }
            }
            else
            {
                // Add give
                FsmState get = fsm.GetState("Get");
                List<FsmStateAction> fsmActions = get.Actions.ToList();
                fsmActions.RemoveAt(fsmActions.Count - 1); // SendEventByName (essence counter)
                fsmActions.RemoveAt(fsmActions.Count - 1); // PlayerDataIntAdd (add essence)
                fsmActions.Add(new AsyncLambda(GiveAllAsync(fsm.transform)));
                get.SetActions(fsmActions.ToArray());

                FsmState vanishBurst = fsm.GetState("Vanish Burst");
                if (vanishBurst != null 
                    && vanishBurst.Actions.Length > 0
                    && vanishBurst.Actions[^1] is SendEventByName sebn
                    && sebn.sendEvent.Value == "DREAM NAIL HUD")
                {
                    vanishBurst.RemoveAction(vanishBurst.Actions.Length - 1);
                }
            }
        }
    }
}
