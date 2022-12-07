using HutongGames.Utility;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger
{
    public abstract class CostDisplayer
    {
        public virtual ISprite CustomCostSprite { get; set; } = null;

        public virtual bool Cumulative { get; set; } = false;

        public int GetDisplayAmount(Cost cost)
        {
            Log($"Getting display amount for {cost}");
            Cost baseCost = cost.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                IEnumerable<Cost> validCosts = mc.Where(SupportsCost);
                if (Cumulative)
                {
                    return validCosts.Max(GetSingleCostDisplayAmount);
                }
                else
                {
                    return validCosts.Sum(GetSingleCostDisplayAmount);
                }
            }
            else if (SupportsCost(baseCost))
            {
                return GetSingleCostDisplayAmount(baseCost);
            }
            else
            {
                return 0;
            }
        }

        public string GetAdditionalCostText(Cost cost)
        {
            Cost baseCost = cost.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                return string.Join(Language.Language.Get("COMMA_SPACE", "IC"), mc.Where(c => !SupportsCost(c))
                    .Select(c => c.GetCostText()).ToArray());
            }
            else if (!SupportsCost(baseCost))
            {
                return baseCost.GetCostText();
            }
            else
            {
                return null;
            }
        }

        protected abstract bool SupportsCost(Cost cost);

        protected abstract int GetSingleCostDisplayAmount(Cost cost);
    }

    public class GeoCostDisplayer : CostDisplayer
    {
        public override ISprite CustomCostSprite => null;

        public override bool Cumulative => false;

        protected override bool SupportsCost(Cost cost) => cost is GeoCost;

        protected override int GetSingleCostDisplayAmount(Cost cost)
        {
            GeoCost gc = cost as GeoCost;
            return (int)(gc.amount * gc.DiscountRate);
        }
    }

    public class PDIntCostDisplayer : CostDisplayer
    {
        public string FieldName { get; set; }

        protected override bool SupportsCost(Cost cost)
        {
            if (Cumulative && cost is PDIntCost pdi && pdi.fieldName == FieldName)
            {
                return true;
            }
            else if (!Cumulative && cost is ConsumablePDIntCost cpdi && cpdi.fieldName == FieldName)
            {
                return true;
            }
            return false;
        }

        protected override int GetSingleCostDisplayAmount(Cost cost)
        {
            if (Cumulative)
            {
                return (cost as PDIntCost).amount;
            }
            else
            {
                return (cost as ConsumablePDIntCost).amount;
            }
        }
    }
}
