using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Actions
{
    class EditFsm : RandomizerAction
    {
        readonly string _sceneName;
        readonly string _fsmName;
        readonly string _objectName;
        readonly Action<PlayMakerFSM> _action;

        public EditFsm(string sceneName, string fsmName, string objectName, Action<PlayMakerFSM> action)
        {
            _sceneName = sceneName;
            _fsmName = fsmName;
            _objectName = objectName;
            _action = action;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, UnityEngine.Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            _action(fsm);
        }
    }
}
