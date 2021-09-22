using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Changes the blue hp threshold for the LBC door. TransitionFixes handles opening the door when entering from behind.
    /// </summary>
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
