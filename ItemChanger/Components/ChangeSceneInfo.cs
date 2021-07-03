using UnityEngine;

namespace ItemChanger.Components
{
    public class ChangeSceneInfo : MonoBehaviour
    {
        public const string door_dreamReturn = "door_dreamReturn";

        public string toScene;
        public string toGate = door_dreamReturn;
        public bool applied;
    }
}
