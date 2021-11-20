using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which makes Void Heart unequippable and makes Siblings and the Shade hostile when Void Heart is not equipped.
    /// </summary>
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

        private bool TestVoidHeartEquipped()
        {
            return PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState)) >= 4 && PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_40));
        }

        private void PreventFriendlyShade(PlayMakerFSM fsm)
        {
            if (fsm.gameObject != null && fsm.gameObject.name.StartsWith("Hollow Shade") && !fsm.gameObject.name.StartsWith("Hollow Shade Death"))
            {
                FsmState pause = fsm.GetState("Pause");
                if (pause == null) return;
                pause.RemoveFirstActionOfType<IntCompare>();
                pause.RemoveFirstActionOfType<GetPlayerDataInt>();
                pause.AddFirstAction(new DelegateBoolTest(TestVoidHeartEquipped, null, FsmEvent.Finished));
            }
        }

        private void PreventFriendlySibling(PlayMakerFSM fsm)
        {
            if (fsm.gameObject != null && fsm.gameObject.name.StartsWith("Shade Sibling"))
            {
                FsmState pause = fsm.GetState("Pause");
                if (pause == null) return;

                pause.RemoveFirstActionOfType<GetPlayerDataInt>();
                pause.RemoveFirstActionOfType<IntCompare>();
                pause.InsertAction(new DelegateBoolTest(TestVoidHeartEquipped, null, FsmEvent.Finished), 2);
            }
        }

    }
}
