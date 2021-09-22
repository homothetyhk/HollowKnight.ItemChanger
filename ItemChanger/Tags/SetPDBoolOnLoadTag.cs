using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class SetPDBoolOnLoadTag : Tag
    {
        public string boolName;
        public bool value = true;

        public override void Load(object parent)
        {
            PlayerData.instance.SetBool(boolName, value);
        }
    }
}
