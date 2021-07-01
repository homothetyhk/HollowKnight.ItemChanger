using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    /// <summary>
    /// Dual location which chooses based on SceneData.
    /// </summary>
    public class SDDualLocation : DualLocation
    {
        /// <summary>
        /// Persistent Bool Data ID
        /// </summary>
        public string pbdID;

        public override bool Decide()
        {
            var pbd = GameManager.instance.sceneData.FindMyState(new PersistentBoolData
            {
                id = pbdID,
                sceneName = sceneName,
            });
            return pbd?.activated ?? false;
        }
    }
}
