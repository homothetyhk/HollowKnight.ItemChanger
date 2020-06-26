using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SeanprCore;
using UnityEngine;

namespace ItemChanger.Actions
{
    public class ChangeChestGeo : RandomizerAction
    {
        private readonly ILP _ilp;

        private readonly string _sceneName;
        private readonly string _fsmName;
        private readonly string _objectName;

        internal ChangeChestGeo(ILP ilp, string sceneName, string objectName, string fsmName)
        {
            _ilp = ilp;
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            // Remove actions that activate shiny item
            FsmState spawnItems = fsm.GetState("Spawn Items");
            spawnItems.RemoveActionsOfType<ActivateAllChildren>();
            fsm.GetState("Activated").RemoveActionsOfType<ActivateAllChildren>();

            // Add geo to chest
            // Chest geo pool cannot be trusted, often spawns less than it should
            spawnItems.AddAction(new RandomizerAddGeo(fsm.gameObject, _ilp.item.geo));
            spawnItems.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(_ilp)));

            // Remove pre-existing geo from chest
            foreach (FlingObjectsFromGlobalPool fling in spawnItems.GetActionsOfType<FlingObjectsFromGlobalPool>())
            {
                fling.spawnMin = 0;
                fling.spawnMax = 0;
            }

            // Need to check SpawnFromPool action too because of Mantis Lords chest
            foreach (SpawnFromPool spawn in spawnItems.GetActionsOfType<SpawnFromPool>())
            {
                spawn.spawnMin = 0;
                spawn.spawnMax = 0;
            }
        }
    }
}
