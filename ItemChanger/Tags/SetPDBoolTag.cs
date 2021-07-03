using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class SetPDBoolTag : Tag, IGiveEffectTag
    {
        public string fieldName;
        [System.ComponentModel.DefaultValue(true)]
        public bool setValue = true;

        public void OnGive(ReadOnlyGiveEventArgs args)
        {
            PlayerData.instance.SetBool(fieldName, setValue);
        }
    }
}
