using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which adds setting a PlayerData bool as a side effect to an item.
    /// </summary>
    public class SetPDBoolOnGiveTag : Tag
    {
        public string fieldName;
        public bool setValue = true;

        public override void Load(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.OnGive += OnGive;
        }

        public override void Unload(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.OnGive -= OnGive;
        }

        public void OnGive(ReadOnlyGiveEventArgs args)
        {
            PlayerData.instance.SetBool(fieldName, setValue);
        }
    }
}
