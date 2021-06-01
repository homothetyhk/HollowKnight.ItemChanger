using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class MapItem : AbstractItem
    {
        public string fieldName;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasMap), true);
            PlayerData.instance.SetBool(nameof(PlayerData.openedMapperShop), true);
            PlayerData.instance.SetBool(fieldName, true);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(fieldName);
        }

    }
}
