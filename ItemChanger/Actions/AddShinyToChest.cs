using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using UnityEngine;

namespace ItemChanger.Actions
{
    public class AddShinyToChest : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _newShinyName;
        private readonly string _objectName;
        private readonly string _sceneName;

        public AddShinyToChest(string sceneName, string objectName, string fsmName, string newShinyName)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _newShinyName = newShinyName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            FsmState spawnItems = fsm.GetState("Spawn Items");

            // Remove geo from chest
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

            // Instantiate a new shiny and set the chest as its parent
            GameObject item = fsm.gameObject.transform.Find("Item").gameObject;
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.SetActive(false);
            shiny.transform.SetParent(item.transform);
            shiny.transform.position = item.transform.position;
            shiny.name = _newShinyName;

            // Force the new shiny to fling out of the chest
            PlayMakerFSM shinyControl = FSMUtility.LocateFSM(shiny, "Shiny Control");
            FsmState shinyFling = shinyControl.GetState("Fling?");
            shinyFling.ClearTransitions();
            shinyFling.AddTransition("FINISHED", "Fling R");
        }
    }
}