using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class StartLocation : AbstractLocation
    {
        [System.ComponentModel.DefaultValue(MessageType.Corner)]
        public MessageType MessageType = MessageType.Corner;

        public override void OnLoad()
        {
            base.OnLoad();
        }
        public override void OnNextSceneReady(Scene next)
        {
            base.OnNextSceneReady(next);
            if (GameManager.instance?.IsGameplayScene() ?? false)
            {
                WaitAndGive();
            }
        }

        private void WaitAndGive()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.OnFinishedEnteringScene += GiveHook;
            }
            else Placement.GiveAll(MessageType);
        }

        private void GiveHook()
        {
            Placement.GiveAll(MessageType);
            GameManager.instance.OnFinishedEnteringScene -= GiveHook;
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.StartPlacement
            {
                location = this,
            };
        }
    }
}
