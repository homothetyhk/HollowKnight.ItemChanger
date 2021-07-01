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
        public Cost Cost
        {
            get => item.GetTag<CostTag>()?.Cost;
            set => item.GetOrAddTag<CostTag>().Cost = Cost;
        }
    }
}
