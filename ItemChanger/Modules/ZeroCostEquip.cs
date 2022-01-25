using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module that allows 0 cost charms to be equipped at any time.
    /// </summary>
    [DefaultModule]
    public class ZeroCostCharmEquip : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(new("Charms", "UI Charms"), AllowZeroCostEquip);
        }
        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Charms", "UI Charms"), AllowZeroCostEquip);
        }

        private void AllowZeroCostEquip(PlayMakerFSM fsm)
        {
            FsmState slotOpen = fsm.GetState("Slot Open?");

            FsmStateAction CheckCanEquip = new DelegateBoolTest(CanEquip, null, "CANCEL");

            slotOpen.Actions = new[]
            {
                slotOpen.Actions[0],
                slotOpen.Actions[1],
                slotOpen.Actions[2],
                CheckCanEquip
            };

            bool CanEquip()
            {
                if (PlayerData.instance.GetInt(fsm.FsmVariables.GetFsmString("PlayerData Var Name").Value) == 0)
                {
                    return true;
                }
                else
                {
                    int notchesUsed = PlayerData.instance.GetInt(nameof(PlayerData.charmSlotsFilled));
                    int notches = PlayerData.instance.GetInt(nameof(PlayerData.charmSlots));
                    return notchesUsed < notches;
                }
            }
        }
    }
}
