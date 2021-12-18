using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which causes dreamnailing the Kingsmould corpse in Palace Grounds to always warp the knight to White_Palace_11.
    /// </summary>
    public class DisablePalaceMidWarp : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Abyss_05, new("Dream Enter 1", "FSM"), AllowRegularPalaceEntry);
            Events.AddFsmEdit(SceneNames.Abyss_05, new("Dream Enter 2", "Check if midwarp or completed"), PreventPalaceMidWarp);
        }
        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Abyss_05, new("Dream Enter 1", "FSM"), AllowRegularPalaceEntry);
            Events.RemoveFsmEdit(SceneNames.Abyss_05, new("Dream Enter 2", "Check if midwarp or completed"), PreventPalaceMidWarp);
        }

        private void PreventPalaceMidWarp(PlayMakerFSM fsm)
        {
            fsm.GetState("Check").RemoveActionsOfType<PlayerDataBoolTest>();
        }

        private void AllowRegularPalaceEntry(PlayMakerFSM fsm)
        {
            if (fsm.FsmVariables.FindFsmString("playerData bool").Value != nameof(PlayerData.dreamNailUpgraded))
            {
                fsm.GetState("Check").RemoveTransitionsTo("Destroy");
            }
        }
    }
}