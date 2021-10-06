using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Components
{
    public class ModShopItemStats : MonoBehaviour
    {
        public AbstractItem item;
        public AbstractPlacement placement;
        public UIDef UIDef => item.GetResolvedUIDef(placement);
        public Cost cost
        {
            get => item.GetTag<CostTag>()?.Cost;
            set => item.GetOrAddTag<CostTag>().Cost = cost;
        }
    }
}
