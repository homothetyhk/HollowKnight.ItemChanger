using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SeanprCore;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace ItemChanger.Actions
{
    internal class ChangeBoolTest : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _stateName;
        private readonly Func<bool> _altTest;

        public ChangeBoolTest(string sceneName, string objectName, string fsmName, string stateName, Func<bool> altTest)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _stateName = stateName;
            _altTest = altTest;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            PlayerDataBoolTest pdBoolTest = fsm.GetState(_stateName).GetActionsOfType<PlayerDataBoolTest>()[0];
            FsmEvent trueEvent = pdBoolTest.isTrue;
            string trueName = trueEvent != null ? trueEvent.Name : null;
            FsmEvent falseEvent = pdBoolTest.isFalse;
            string falseName = falseEvent != null ? falseEvent.Name : null;

            fsm.GetState(_stateName).AddFirstAction(
            new RandomizerExecuteLambda(() => fsm.SendEvent(
                _altTest() ? trueName : falseName
                )));
            fsm.GetState(_stateName).RemoveActionsOfType<PlayerDataBoolTest>();
        }
    }
}
