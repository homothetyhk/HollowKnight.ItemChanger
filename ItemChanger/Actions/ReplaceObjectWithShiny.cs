using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Actions
{
    public class ReplaceObjectWithShiny : RandomizerAction
    {
        private readonly string _newShinyName;
        private readonly string _objectName;
        private readonly string _sceneName;

        public ReplaceObjectWithShiny(string sceneName, string objectName, string newShinyName)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _newShinyName = newShinyName;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject obj = currentScene.FindGameObject(_objectName);

            if (obj == null)
            {
                Modding.Logger.LogWarn($"GameObject {_objectName} was not found in {_sceneName}");
                return;
            }
            
            // Put a shiny in the same location as the original
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = _newShinyName;
            if (obj.transform.parent != null)
            {
                shiny.transform.SetParent(obj.transform.parent);
            }

            shiny.transform.position = obj.transform.position;
            shiny.transform.localPosition = obj.transform.localPosition;
            shiny.SetActive(obj.activeSelf);

            // Force the new shiny to fall straight downwards
            PlayMakerFSM fsm = FSMUtility.LocateFSM(shiny, "Shiny Control");
            FsmState fling = fsm.GetState("Fling?");
            fling.ClearTransitions();
            fling.AddTransition("FINISHED", "Fling R");
            FlingObject flingObj = fsm.GetState("Fling R").GetActionsOfType<FlingObject>()[0];
            flingObj.angleMin = flingObj.angleMax = 270;

            // For some reason not setting speed manually messes with the object position
            flingObj.speedMin = flingObj.speedMax = 0.1f;

            // Destroy the original
            Object.Destroy(obj);
        }
    }
}