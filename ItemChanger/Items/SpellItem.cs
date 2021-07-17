using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class SpellItem : AbstractItem
    {
        public string fieldName;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasSpell), true);
            PlayerData.instance.IncrementInt(fieldName);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetInt(fieldName) > 2;
        }
    }
}
