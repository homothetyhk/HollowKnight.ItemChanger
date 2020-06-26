using HutongGames.PlayMaker;
using SeanprCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Actions
{
    public class AddFsmActionToObject : RandomizerAction
    {
        readonly string _sceneName;
        readonly string _fsmName;
        readonly string _objectName;
        readonly FsmStateAction _action;
        readonly string[] _stateNames;
        readonly bool _first;

        public AddFsmActionToObject(string sceneName, string fsmName, string objectName, bool first, FsmStateAction action, params string[] stateNames)
        {
            _sceneName = sceneName;
            _fsmName = fsmName;
            _objectName = objectName;
            _action = action;
            _stateNames = stateNames;
            _first = first;
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
                    if (_first)
                    {
                        state.AddFirstAction(_action);
                    }
                    else
                    {
                        state.AddAction(_action);
                    }
                }
            }
        }
    }
}
