using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    internal static class AdditiveManager
    {
        private static Dictionary<string, _ILP[]> additiveGroups;

        public static void Setup()
        {
            additiveGroups = new Dictionary<string, _ILP[]>();
            foreach (_ILP ilp in _ILP.ILPs.Values)
            {
                string key = ilp.item.additiveGroup;
                if (string.IsNullOrEmpty(key)) continue;
                if (additiveGroups.ContainsKey(key)) continue;
                List<_ILP> group = _ILP.ILPs.Values.Where(i => i.item.additiveGroup == key).ToList();
                group.Sort((i1, i2) => i1.item.additiveIndex - i2.item.additiveIndex);
                additiveGroups[key] = group.ToArray();
            }
        }

        public static int GetAdditiveCount(_Item item)
        {
            if (string.IsNullOrEmpty(item.additiveGroup)) 
            {
                return 0;
            }
            return additiveGroups[item.additiveGroup].Count(i => ItemChanger.instance.Settings.CheckObtained(i.id));
        }

        public static _ILP GetNextAdditiveItem(_ILP item)
        {
            if (string.IsNullOrEmpty(item.item.additiveGroup)) return item;

            int additiveCount = GetAdditiveCount(item.item);
            if (additiveCount < additiveGroups[item.item.additiveGroup].Length)
            {
                return additiveGroups[item.item.additiveGroup][additiveCount];
            }
            return additiveGroups[item.item.additiveGroup].Last();
        }
    }
}
