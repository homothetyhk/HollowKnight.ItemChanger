﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class RelicItem : AbstractItem
    {
        public int trinketNum;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool($"foundTrinket{trinketNum}", true);
            PlayerData.instance.IncrementInt($"trinket{trinketNum}");
        }

    }
}
