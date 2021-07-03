using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Tags
{
    public class PDIntShopReqTag : Tag, IShopRequirementTag
    {
        public string fieldName;
        public int threshold;
        [System.ComponentModel.DefaultValue(ComparisonOperator.Ge)]
        public ComparisonOperator op = ComparisonOperator.Ge;

        public bool MeetsRequirement()
        {
            return PlayerData.instance.GetInt(fieldName).Compare(op, threshold);
        }
    }
}
