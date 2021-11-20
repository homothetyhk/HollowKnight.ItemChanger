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
        public string objName;

        // TODO: change bool test, so that location can be checked multiple times if necessary

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
            FsmState get = fsm.GetState("Get");
            List<FsmStateAction> fsmActions = get.Actions.ToList();
            fsmActions.RemoveAt(fsmActions.Count - 1); // SendEventByName (essence counter)
            fsmActions.RemoveAt(fsmActions.Count - 1); // PlayerDataIntAdd (add essence)
            fsmActions.Add(new AsyncLambda(GiveAllAsync(fsm.transform)));
            get.Actions = fsmActions.ToArray();
        }
    }
}
