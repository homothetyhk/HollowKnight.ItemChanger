using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class StagItem : AbstractItem
    {
        public string fieldName;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            PlayerData.instance.SetBool(fieldName, true);
            PlayerData.instance.IncrementInt(nameof(PlayerData.stationsOpened));
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(fieldName);
        }
    }
}
