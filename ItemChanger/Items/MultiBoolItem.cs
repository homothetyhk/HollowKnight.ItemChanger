using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which sets each of the provided PlayerData fields to true.
    /// </summary>
    public class MultiBoolItem : AbstractItem
    {
        public string[] fieldNames;

        public override void GiveImmediate(GiveInfo info)
        {
            foreach (var field in fieldNames) PlayerData.instance.SetBool(field, true);
        }

        public override bool Redundant()
        {
            return fieldNames.All(field => PlayerData.instance.GetBool(field));
        }
    }
}
