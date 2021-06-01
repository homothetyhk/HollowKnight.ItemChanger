using GlobalEnums;
using HutongGames.PlayMaker;
using Modding;
using SereCore;

namespace ItemChanger.FsmStateActions
{
    internal class RandomizerChangeScene : FsmStateAction
    {
        private readonly string _gateName;

        private readonly string _sceneName;

        public RandomizerChangeScene(string scene, string gate)
        {
            _sceneName = scene;
            _gateName = gate;
        }

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(_sceneName) && !string.IsNullOrEmpty(_gateName))
            {
                ChangeToScene(_sceneName, _gateName);
            }

            Finish();
        }

        private void ChangeToScene(string sceneName, string gateName, float delay = 0f)
        {
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(gateName))
            {
                Log("Empty string passed into ChangeToScene, ignoring");
                return;
            }

            SceneLoad load = ReflectionHelper.GetAttr<GameManager, SceneLoad>(SereCore.Ref.GM, "sceneLoad");
            if (load != null)
            {
                load.Finish += () =>
                {
                    LoadScene(sceneName, gateName, delay);
                };
            }
            else
            {
                LoadScene(sceneName, gateName, delay);
            }
        }

        private static void LoadScene(string sceneName, string gateName, float delay)
        {
            SereCore.Ref.GM.StopAllCoroutines();
            ReflectionHelper.SetAttr<GameManager, SceneLoad>(SereCore.Ref.GM, "sceneLoad", null);

            SereCore.Ref.GM.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                IsFirstLevelForPlayer = false,
                SceneName = sceneName,
                HeroLeaveDirection = GetGatePosition(gateName),
                EntryGateName = gateName,
                EntryDelay = delay,
                PreventCameraFadeOut = false,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false
            });
        }

        private static GatePosition GetGatePosition(string name)
        {
            if (name.Contains("top"))
            {
                return GatePosition.top;
            }

            if (name.Contains("bot"))
            {
                return GatePosition.bottom;
            }

            if (name.Contains("left"))
            {
                return GatePosition.left;
            }

            if (name.Contains("right"))
            {
                return GatePosition.right;
            }

            if (name.Contains("door"))
            {
                return GatePosition.door;
            }

            return GatePosition.unknown;
        }
    }
}