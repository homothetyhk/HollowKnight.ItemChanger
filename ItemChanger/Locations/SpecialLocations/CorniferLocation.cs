using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which gives items through the interface to purchase a map from Cornifer.
    /// </summary>
    public class CorniferLocation : AutoLocation
    {
        public string objectName;
        public override bool SupportsCost => true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, "Conversation Control"), HandleCorniferConvo);
            Events.AddFsmEdit(sceneName, new("Cornifer Card", "FSM"), HandleCorniferCard);
            Events.AddLanguageEdit(new("Cornifer", "CORNIFER_PROMPT"), OnLanguageGet);
            ItemChangerMod.Modules.GetOrAdd<Modules.AltCorniferAtHomeTest>().Add(this);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, "Conversation Control"), HandleCorniferConvo);
            Events.RemoveFsmEdit(sceneName, new("Cornifer Card", "FSM"), HandleCorniferCard);
            Events.RemoveLanguageEdit(new("Cornifer", "CORNIFER_PROMPT"), OnLanguageGet);
        }

        private void HandleCorniferConvo(PlayMakerFSM fsm)
        {
            // If an item respawned after paying Cornifer, we destroy Cornifer and spawn a shiny
            // If all of the items are obtained, the checkActive action will destroy Cornifer without spawning a shiny
            if (!IsCorniferPresent() && !Placement.AllObtained())
            {
                Container c = Container.GetContainer(Container.Shiny);
                GameObject shiny = c.GetNewContainer(new ContainerInfo(c.Name, Placement, flingType, (Placement as Placements.ISingleCostPlacement)?.Cost));
                c.ApplyTargetContext(shiny, fsm.gameObject, 0);
                ShinyUtility.FlingShinyDown(shiny.LocateMyFSM("Shiny Control"));
                UnityEngine.Object.Destroy(fsm.gameObject);
                return;
            }

            FsmState checkActive = fsm.GetState("Check Active");
            FsmState convoChoice = fsm.GetState("Convo Choice");
            FsmState get = fsm.GetState("Geo Pause and GetMap");
            FsmState sendText = fsm.GetState("Send Text");

            checkActive.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)checkActive.Actions[0]);
            convoChoice.Actions[1] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)convoChoice.Actions[1]);

            get.Actions = new FsmStateAction[]
            {
                get.Actions[0], // Wait
                get.Actions[1], // Box Down
                get.Actions[2], // Npc title down
                // get.Actions[3] // SetPlayerDataBool
                // get.Actions[4-7] // nonDeepnest only, map achievement/messages
                new Lambda(() => Placement.AddVisitFlag(VisitState.Accepted)),
                new AsyncLambda(GiveAllAsync(fsm.transform), "TALK FINISH"),
            };
            get.ClearTransitions();

            // If the placement declares a different cost, we overwrite the map cost
            // otherwise, Cornifer charges the vanilla price
            if (Placement is Placements.ISingleCostPlacement iscp && iscp.Cost is Cost cost)
            {
                sendText.ClearActions();
                sendText.AddFirstAction(new Lambda(() => YNUtil.OpenYNDialogue(fsm.gameObject, Placement, Placement.Items, cost)));
                get.AddFirstAction(new Lambda(() => { if (!cost.Paid) cost.Pay(); }));
            }

            if (fsm.GetState("Deepnest Check") is FsmState deepnestCheck)
            {
                deepnestCheck.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)deepnestCheck.Actions[0]);
            }
        }

        private void HandleCorniferCard(PlayMakerFSM fsm)
        {
            FsmState check = fsm.GetState("Check");
            check.Actions[0] = new DelegateBoolTest(IsCorniferCardPresent, (PlayerDataBoolTest)check.Actions[0]);
        }

        public bool IsCorniferPresent() => !Placement.CheckVisitedAll(VisitState.Accepted) && !Placement.AllObtained(); // we set the special flag after purchasing from Cornifer
        public bool IsCorniferCardPresent() => Placement.AllObtained();

        private void OnLanguageGet(ref string value)
        {
            if (GameManager.instance.sceneName == sceneName)
            {
                value = Placement.GetUIName();
                Placement.OnPreview(value);
            }
        }
    }
}
