using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using UnityEngine;

namespace ItemChanger.Actions
{
    internal class CreateNewShiny : RandomizerAction
    {
        private readonly string _newShinyName;
        private readonly string _sceneName;
        private readonly float _x;
        private readonly float _y;

        public CreateNewShiny(string sceneName, float x, float y, string newShinyName)
        {
            _sceneName = sceneName;
            _x = x;
            _y = y;
            _newShinyName = newShinyName;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            // Put a shiny in the same location as the original
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = _newShinyName;

            shiny.transform.position = new Vector3(_x, _y, shiny.transform.position.z);
            
            shiny.SetActive(true);

            // Force the new shiny to fall straight downwards
            PlayMakerFSM fsm = FSMUtility.LocateFSM(shiny, "Shiny Control");
            FsmState fling = fsm.GetState("Fling?");
            fling.ClearTransitions();
            fling.AddTransition("FINISHED", "Fling R");
            FlingObject flingObj = fsm.GetState("Fling R").GetActionsOfType<FlingObject>()[0];
            flingObj.angleMin = flingObj.angleMax = 270;

            // For some reason not setting speed manually messes with the object position
            flingObj.speedMin = flingObj.speedMax = 0.1f;
        }
    }
}
