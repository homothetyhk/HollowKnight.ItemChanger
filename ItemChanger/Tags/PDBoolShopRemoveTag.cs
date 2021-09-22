using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class PDBoolShopRemoveTag : Tag, IShopRemovalTag
    {
        public string fieldName;
        [System.ComponentModel.DefaultValue(true)]
        public bool removeVal = true;

        public bool Remove => PlayerData.instance.GetBool(fieldName) == removeVal;
    }
}
