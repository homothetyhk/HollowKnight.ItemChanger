using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which directly gives items after interacting with the vanilla Shade Soul location.
    /// </summary>
    public class ShadeSoulLocation : AutoLocation
    {
        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("Ruins Shaman", "Ruins Shaman"), EditRuinsShaman);
            Events.AddFsmEdit(sceneName, new("Knight Get Fireball Lv2", "Get Fireball"), EditGetFireball);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("Ruins Shaman", "Ruins Shaman"), EditRuinsShaman);
            Events.RemoveFsmEdit(sceneName, new("Knight Get Fireball Lv2", "Get Fireball"), EditGetFireball);
        }

        private void EditRuinsShaman(PlayMakerFSM fsm)
        {
            FsmState gotSpell = fsm.GetState("Got Spell?");
            gotSpell.RemoveActionsOfType<IntCompare>();
            gotSpell.AddLastAction(new DelegateBoolTest(Placement.AllObtained, "ACTIVATED", null));
        }

        private void EditGetFireball(PlayMakerFSM fsm)
        {
            FsmState getPD = fsm.GetState("Get PlayerData");
            FsmState UIMsg = fsm.GetState("Call UI Msg");

            FsmStateAction give = new AsyncLambda(GiveAll, "GET ITEM MSG END");

            getPD.RemoveActionsOfType<SetPlayerDataInt>();
            UIMsg.Actions = new FsmStateAction[]
            {
                give
            };
        }
    }
}
