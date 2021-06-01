using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    [Obsolete("Use AbstractPlacement instead.")]
    internal class _ILP
    {
        internal static Dictionary<string, _ILP> ILPs;
        internal static void Clear() => ILPs = new Dictionary<string, _ILP>();

        internal _Item item;
        internal _Location location;
        internal string id;

        internal _ILP(_Item _item, _Location _location, object _id)
        {
            item = _item;
            location = _location;
            id = _id.ToString();
        }

        internal static void Process(List<(_Item, _Location)> preILPs)
        {
            Clear();
            for (int i = 0; i < preILPs.Count; i++)
            {
                ILPs[i.ToString()] = new _ILP(preILPs[i].Item1, preILPs[i].Item2, i);
            }
        }

        public override string ToString()
        {
            return $"Item-Location Pair {id}: {item}, {location}";
        }

    }
}
