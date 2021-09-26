using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class RemoveVoidHeartEffects : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(new("Charms", "UI Charms"), AllowVoidHeartUnequip);
            Events.AddFsmEdit(new("Shade Control"), PreventFriendlyShade);
            Events.AddFsmEdit(new("Control"), PreventFriendlySibling);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Charms", "UI Charms"), AllowVoidHeartUnequip);
            Events.RemoveFsmEdit(new("Shade Control"), PreventFriendlyShade);
            Events.RemoveFsmEdit(new("Control"), PreventFriendlySibling);
        }

        private void AllowVoidHeartUnequip(PlayMakerFSM fsm)
        {
            FsmState equipped = fsm.GetState("Equipped?");
            FsmState setCurrentItemNum = fsm.GetState("Set Current Item Num");

            equipped.RemoveTransitionsTo("Black Charm? 2");
            equipped.AddTransition("EQUIPPED", "Return Points");
            setCurrentItemNum.RemoveTransitionsTo("Black Charm?");
            setCurrentItemNum.AddTransition("FINISHED", "Return Points");
        }

        private void PreventFriendlyShade(PlayMakerFSM fsm)
        {
            if (fsm.gameObject != null && fsm.gameObject.name.StartsWith("Hollow Shade"))
            {
                FsmState pause = fsm.GetState("Pause");
                pause.ClearTransitions();
                pause.AddTransition("FINISHED", "Init");
                fsm.FsmVariables.FindFsmBool("Friendly").Value = false;
            }
        }

        private void PreventFriendlySibling(PlayMakerFSM fsm)
        {
            if (fsm.gameObject != null && fsm.gameObject.name.StartsWith("Shade Sibling"))
            {
                FsmState pause = fsm.GetState("Pause");
                pause.ClearTransitions();
                pause.AddTransition("FINISHED", "Init");
                fsm.FsmVariables.FindFsmBool("Friendly").Value = false;
            }
        }

    }
}
