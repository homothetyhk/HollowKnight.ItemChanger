using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    /// <summary>
    /// BoolItem which unlocks the map and marker panels and opens Iselda.
    /// </summary>
    public class MapMarkerItem : AbstractItem
    {
        public string fieldName;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasMap), true);
            PlayerData.instance.SetBool(nameof(PlayerData.hasMarker), true);
            PlayerData.instance.SetBool(nameof(PlayerData.openedMapperShop), true);
            PlayerData.instance.SetBool(fieldName, true);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(fieldName);
        }
    }
}
