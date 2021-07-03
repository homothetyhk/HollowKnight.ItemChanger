using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class PDBoolShopReqTag : Tag, IShopRequirementTag
    {
        public string fieldName;
        [System.ComponentModel.DefaultValue(true)]
        public bool reqVal = true;

        public bool MeetsRequirement()
        {
            return PlayerData.instance.GetBool(fieldName) == reqVal;
        }
    }
}
