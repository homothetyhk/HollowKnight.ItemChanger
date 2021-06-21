using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
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
