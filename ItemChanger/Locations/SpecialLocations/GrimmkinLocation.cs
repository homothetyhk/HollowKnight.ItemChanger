using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location for modifying the unique Grimmkin spawn of a scene.
    /// </summary>
    public class GrimmkinLocation : AutoLocation
    {
        public int grimmkinLevel;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("Control"), EditFlamebearerControl);
            Events.AddFsmEdit(sceneName, new("Flamebearer Spawn", "Spawn Control"), EditFlamebearerSpawn);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("Control"), EditFlamebearerControl);
            Events.RemoveFsmEdit(sceneName, new("Flamebearer Spawn", "Spawn Control"), EditFlamebearerSpawn);
        }

        public override GiveInfo GetGiveInfo()
        {
            var info = base.GetGiveInfo();
            info.MessageType = MessageType.Corner;
            return info;
        }

        private void EditFlamebearerControl(PlayMakerFSM fsm)
        {
            if (!fsm.gameObject.name.StartsWith("Flamebearer")) return;

            FsmState init = fsm.GetState("Init");
            init.ReplaceAction(new SetIntValue
            {
                intVariable = ((GetPlayerDataInt)init.Actions[2]).storeValue,
                intValue = grimmkinLevel
            }, 2);
        }

        private void EditFlamebearerSpawn(PlayMakerFSM fsm)
        {
            FsmState state = fsm.GetState("State");
            FsmState get = fsm.GetState("Get");

            // Override Grimmchild level check
            state.ClearTransitions();
            state.AddTransition("FINISHED", $"Level {grimmkinLevel}");
            state.AddTransition("KILLED", "Do Nothing");
            bool Check()
            {
                return Placement.AllObtained() || !GrimmchildRequirement();
            }

            state.SetActions(
                new DelegateBoolTest(Check, (BoolTest)state.Actions[0])
            );

            get.SetActions(
                get.Actions[6], // set Activated--not used by IC, but preserves grimmkin status if IC is disabled
                new AsyncLambda(GiveAll)
            );
        }

        public bool GrimmchildRequirement()
        {
            return PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_40)) && PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) >= grimmkinLevel;
        }

    }
}
