using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class StartLocation : AbstractLocation
    {
        public override void OnLoad()
        {
            base.OnLoad();
        }
        public override void OnNextSceneReady(Scene next)
        {
            base.OnNextSceneReady(next);
            if (GameManager.instance?.IsGameplayScene() ?? false)
            {
                Placement.GiveAll(MessageType.Corner); // use the latest scene change hook, so it's most likely to appear onscreen
            }
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
