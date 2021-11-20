using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which changes the blue hp threshold for the LBC door to the specified value. TransitionFixes handles opening the door when entering from behind.
    /// </summary>
    [DefaultModule]
    public class BlueDoorUnlock : Module
    {
        public int blueHealthThreshold = 1;

        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Abyss_06_Core, new("Blue Door", "Control"), ModifyBlueDoorRequirement);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Abyss_06_Core, new("Blue Door", "Control"), ModifyBlueDoorRequirement);
        }

        private void ModifyBlueDoorRequirement(PlayMakerFSM fsm)
        {
            int blueHealth = PlayerData.instance.GetInt(nameof(PlayerData.healthBlue));
            int joniHealth = PlayerData.instance.GetInt(nameof(PlayerData.joniHealthBlue));
            if (blueHealth + joniHealth < blueHealthThreshold) return;

            FsmState init = fsm.GetState("Init");
            FsmState opened = fsm.GetState("Opened");
            foreach (FsmTransition t in init.Transitions)
            {
                t.SetToState(opened);
            }
            PlayerData.instance.SetBool(nameof(PlayerData.blueVineDoor), true);
        }
    }
}
