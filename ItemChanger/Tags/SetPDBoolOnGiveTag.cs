using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class SetPDBoolOnGiveTag : Tag
    {
        public string fieldName;
        [System.ComponentModel.DefaultValue(true)]
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
