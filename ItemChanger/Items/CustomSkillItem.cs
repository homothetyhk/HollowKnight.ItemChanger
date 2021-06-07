using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    public class CustomSkillItem : AbstractItem
    {
        public string boolName;
        public override void GiveImmediate(Container container, FlingType fling, Transform transform)
        {
            Ref.SKILLS.SetBool(boolName, true);
        }

        public override bool Redundant()
        {
            return Ref.SKILLS.GetBool(boolName);
        }

    }
}
