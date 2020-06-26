using ItemChanger.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Actions
{
    public class CreateObjectDestroyer : RandomizerAction
    {
        readonly string _sceneName;
        readonly string _destroyName;

        public CreateObjectDestroyer(string sceneName, string destroyName)
        {
            _sceneName = sceneName;
            _destroyName = destroyName;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, UnityEngine.Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            ObjectDestroyer.Destroy(_sceneName, _destroyName);

        }
    }
}
