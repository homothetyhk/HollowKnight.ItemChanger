using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Subtractive rancid egg cost which adjusts for the number of eggs previously spent on this type of cost.
    /// </summary>
    public class CumulativeRancidEggCost : Cost
    {
        public int Total;
        public int Balance => Total - ItemChangerMod.Modules.GetOrAdd<CumulativeEggCostModule>().CumulativeEggsSpent;

        public override bool CanPay() => Balance <= PlayerData.instance.GetInt(nameof(PlayerData.rancidEggs));

        public override void OnPay()
        {
            int bal = Balance;
            if (bal > 0)
            {
                PlayerData.instance.IntAdd(nameof(PlayerData.rancidEggs), -bal);
                ItemChangerMod.Modules.GetOrAdd<CumulativeEggCostModule>().CumulativeEggsSpent += bal;
            }
        }

        public override string GetCostText()
        {
            int bal = Balance;
            if (bal > 0) return $"Pay {bal} rancid " + (bal != 1 ? "eggs." : "egg.");
            else return "Free";
        }
    }

    /// <summary>
    /// Module which tracks the number of rancid eggs that have been spent via CumulativeRancidEggCost.
    /// </summary>
    [DefaultModule]
    public class CumulativeEggCostModule : Module
    {
        public int CumulativeEggsSpent;

        public override void Initialize() { }

        public override void Unload() { }
    }
}
