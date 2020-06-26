using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    internal class ILP
    {
        internal static Dictionary<string, ILP> ILPs;
        internal static void Clear() => ILPs = new Dictionary<string, ILP>();

        internal Item item;
        internal Location location;
        internal string id;

        internal ILP(Item _item, Location _location, object _id)
        {
            item = _item;
            location = _location;
            id = _id.ToString();
        }

        internal static void Process(List<(Item, Location)> preILPs)
        {
            Clear();
            for (int i = 0; i < preILPs.Count; i++)
            {
                ILPs[i.ToString()] = new ILP(preILPs[i].Item1, preILPs[i].Item2, i);
            }
        }

        public override string ToString()
        {
            return $"Item-Location Pair {id}: {item}, {location}";
        }

    }
}
