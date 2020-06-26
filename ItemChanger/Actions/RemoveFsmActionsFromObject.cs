using HutongGames.PlayMaker;
using SeanprCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Actions
{
    public class RemoveFsmActionsFromObject<T> : RandomizerAction where T : FsmStateAction
    {
        readonly string _sceneName;
        readonly string _fsmName;
        readonly string _objectName;
        readonly string[] _stateNames;

        public RemoveFsmActionsFromObject(string sceneName, string fsmName, string objectName, params string[] stateNames)
        {
            _sceneName = sceneName;
            _fsmName = fsmName;
            _objectName = objectName;
            _stateNames = stateNames;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, UnityEngine.Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            foreach (string stateName in _stateNames)
            {
                if (fsm.GetState(stateName) is FsmState state)
                {
                    state.RemoveActionsOfType<T>();
                }
            }
        }
    }
}
