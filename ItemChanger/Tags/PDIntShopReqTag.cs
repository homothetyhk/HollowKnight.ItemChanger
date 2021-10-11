using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Extensions;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which indicates that an item should only appear in the shop's stock if the specified PlayerData int comparison succeeds.
    /// </summary>
    public class PDIntShopReqTag : Tag, IShopRequirementTag
    {
        public string fieldName;
        public int threshold;
        public ComparisonOperator op = ComparisonOperator.Ge;

        public bool MeetsRequirement => PlayerData.instance.GetInt(fieldName).Compare(op, threshold);
    }
}
