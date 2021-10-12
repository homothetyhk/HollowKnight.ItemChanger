using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which increments a spell level and sets the hasSpell bool.
    /// </summary>
    public class SpellItem : AbstractItem
    {
        public string fieldName;
        public int spellLevel;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasSpell), true);
            PlayerData.instance.SetInt(fieldName, spellLevel);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetInt(fieldName) >= spellLevel;
        }
    }
}
