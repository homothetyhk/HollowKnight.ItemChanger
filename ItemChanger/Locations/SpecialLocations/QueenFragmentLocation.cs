using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Locations.SpecialLocations
{
    public class QueenFragmentLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Queen Item", "Control"), EditQueenItem);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Queen Item", "Control"), EditQueenItem);
        }

        private void EditQueenItem(PlayMakerFSM fsm)
        {
            FsmState idle = fsm.GetState("Idle");
            idle.ClearActions();
            idle.AddFirstAction(new DelegateBoolTest(InactiveItem, FsmEvent.GetFsmEvent("INACTIVE"), null));

            FsmState inactive = fsm.GetState("Inactive");
            inactive.AddFirstAction(new ActivateAllChildren { activate = true, gameObject = fsm.FsmVariables.FindFsmGameObject("Self") });

            // normally, idle checks "gotQueenFragment" to go to Inactive and "metQueen" to go to Drop
            // with broadcasting "CHARM DROP" also going to Drop

            // now, we go to Inactive any time after first meeting the Queen, and have Inactive and Drop both spawn the item
            // however, Inactive does not deactivate the Queen's collider, so that it is possible to talk to her on revisiting
        }

        private bool InactiveItem() => PlayerData.instance.GetBool(nameof(PlayerData.metQueen));
    }
}
