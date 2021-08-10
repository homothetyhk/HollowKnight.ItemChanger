using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class SetPDBoolOnLoadTag : Tag, IOnLoadActionTag
    {
        public string boolName;
        public bool value = true;

        // TODO: implement in Placement.OnLoad
        public void OnLoad()
        {
            PlayerData.instance.SetBool(boolName, value);
        }

    }
}
