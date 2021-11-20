using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which gives items directly upon shattering the Crystal Shaman.
    /// </summary>
    public class DescendingDarkLocation : AutoLocation
    {
        public string objectName;
        public string fsmName;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), EditCrystalShaman);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), EditCrystalShaman);
        }

        private void EditCrystalShaman(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            FsmState get = fsm.GetState("Get PlayerData 2");
            FsmState callUI = fsm.GetState("Call UI Msg 2");

            FsmStateAction check = new DelegateBoolTest(Placement.AllObtained, "BROKEN", null);
            FsmStateAction give = new AsyncLambda(GiveAllAsync(fsm.transform), "GET ITEM MSG END");

            init.RemoveActionsOfType<IntCompare>();
            init.AddLastAction(check);

            get.Actions = new FsmStateAction[0];
            callUI.Actions = new[] { give };
        }
    }
}
