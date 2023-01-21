using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location with directly gives items after entering the Shade Cloak dish platform.
    /// </summary>
    public class ShadeCloakLocation : AutoLocation
    {
        protected override void OnLoad()
        {
            Events.AddFsmEdit(UnsafeSceneName, new("Dish Plat", "Get Shadow Dash"), EditDashPlat);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(UnsafeSceneName, new("Dish Plat", "Get Shadow Dash"), EditDashPlat);
        }

        private void EditDashPlat(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.ReplaceAction(new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)init.Actions[0]), 0);

            FsmState takeControl = fsm.GetState("Take Control");
            takeControl.RemoveActionsOfType<ActivateGameObject>();

            FsmState setRespawn = fsm.GetState("Set Respawn");
            setRespawn.ClearActions();

            FsmState pd = fsm.GetState("PlayerData");
            pd.ClearActions();

            FsmState uiMsg = fsm.GetState("UI Msg");
            FsmStateAction give = new AsyncLambda(GiveAllAsync(fsm.transform), "GET ITEM MSG END");
            uiMsg.SetActions(give);

            FsmState end = fsm.GetState("End");
            end.RemoveActionsOfType<SendEventToRegister>();
        }
    }
}
