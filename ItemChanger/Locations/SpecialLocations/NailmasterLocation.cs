using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which gives items when a Nailmaster would teach their nail art.
    /// </summary>
    public class NailmasterLocation : AutoLocation
    {
        public string objectName;
        public string fsmName;
        public override bool SupportsCost => true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), EditNailmasterConvo);
            Events.AddLanguageEdit(new("Prompts", "NAILMASTER_FREE"), OnLanguageGet);
            Events.AddSceneChangeEdit(sceneName, MakeShinyForRespawnedItems);
            if (sceneName == SceneNames.Room_nailmaster_02) ItemChangerMod.Modules.GetOrAdd<Modules.AltNailsmithSheoTest>().Subscribe(this);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), EditNailmasterConvo);
            Events.RemoveLanguageEdit(new("Prompts", "NAILMASTER_FREE"), OnLanguageGet);
            Events.RemoveSceneChangeEdit(sceneName, MakeShinyForRespawnedItems);
        }

        private void EditNailmasterConvo(PlayMakerFSM fsm)
        {
            FsmState convo = fsm.GetState("Convo Choice");
            FsmState getMsg = fsm.GetState("Get Msg");
            FsmState fade = fsm.GetState("Fade Back");
            FsmState sendText = fsm.GetState("Send Text");


            FsmStateAction test = new DelegateBoolTest(MakeOffer, "REOFFER", null);
            FsmStateAction give = new AsyncLambda(GiveAllAsync(fsm.transform), "GET ITEM MSG END");

            convo.Actions[objectName == "NM Sheo NPC" ? 2 : 1] = test;

            getMsg.Actions = new FsmStateAction[]
            {
                getMsg.Actions[0],
                getMsg.Actions[1],
                getMsg.Actions[2],
                new Lambda(() => Placement.AddVisitFlag(VisitState.Accepted)),
                give,
            };

            if (Placement is Placements.ISingleCostPlacement iscp && iscp.Cost is Cost cost)
            {
                sendText.ClearActions();
                sendText.AddFirstAction(new Lambda(() => YNUtil.OpenYNDialogue(fsm.gameObject, Placement.Items, cost)));
                getMsg.AddFirstAction(new Lambda(() => { if (!cost.Paid) cost.Pay(); }));
            }

            sendText.AddFirstAction(new Lambda(() => Placement.AddVisitFlag(VisitState.Previewed)));
        }

        private void MakeShinyForRespawnedItems(Scene to)
        {
            if (AcceptedOffer() && !Placement.AllObtained())
            {
                GameObject nailmaster = to.FindGameObjectByName(objectName);
                Container c = Container.GetContainer(Container.Shiny);
                GameObject shiny = c.GetNewContainer(Placement, Placement.Items, flingType, (Placement as Placements.ISingleCostPlacement)?.Cost);
                c.ApplyTargetContext(shiny, nailmaster.transform.position.x, nailmaster.transform.position.y, 1.5f);
                ShinyUtility.FlingShinyLeft(shiny.LocateFSM("Shiny Control"));
            }
        }

        public bool AcceptedOffer() => Placement.CheckVisitedAll(VisitState.Accepted);
        public bool MakeOffer() => !AcceptedOffer() && !Placement.AllObtained();

        private void OnLanguageGet(ref string value)
        {
            if (GameManager.instance.sceneName == sceneName) value = Placement.GetUIName();
        }
    }
}
