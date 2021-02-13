using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Actions
{
    public class AddSendEventToObject : RandomizerAction
    {
        readonly string _sceneName;
        readonly string _fsmName;
        readonly string _objectName;
        readonly string _eventName;
        readonly string[] _stateNames;

        public AddSendEventToObject(string sceneName, string fsmName, string objectName, string eventName, params string[] stateNames)
        {
            _sceneName = sceneName;
            _fsmName = fsmName;
            _objectName = objectName;
            _eventName = eventName;
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

            SendEvent sender = new SendEvent
            {
                eventTarget = new FsmEventTarget
                {
                    target = FsmEventTarget.EventTarget.BroadcastAll,
                    excludeSelf = true
                },
                sendEvent = FsmEvent.FindEvent(_eventName) ?? new FsmEvent(_eventName),
                delay = 0,
                everyFrame = false
            };

            foreach (string stateName in _stateNames)
            {
                if (fsm.GetState(stateName) is FsmState state)
                {
                    state.AddFirstAction(sender);
                }
            }
        }
    }
}
