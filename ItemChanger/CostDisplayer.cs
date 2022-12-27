using ItemChanger.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemChanger
{
    /// <summary>
    /// A utility object which dictates how costs (and especially multicosts) are to be displayed
    /// in non-textual contexts such as shops. Accounts for multicosts and other correctly-implemented
    /// nested costs.
    /// </summary>
    public abstract class CostDisplayer
    {
        /// <summary>
        /// A sprite to use to display the cost visually, if contextually applicable. If no sprite
        /// is provided, existing sprites won't be replaced. Default is null.
        /// </summary>
        public virtual ISprite CustomCostSprite { get; set; } = null;

        /// <summary>
        /// Whether nested costs should be considered cumulative. A cost is cumulative if costs paid
        /// also count towards subsequent costs. For example, grub costs are cumulative, geo costs are not.
        /// </summary>
        public virtual bool Cumulative { get; set; } = false;

        /// <summary>
        /// Gets the amount to display alongside the cost, e.g. in the shop item list.  For multicosts, 
        /// the maximum amount for single matching costs is displayed if the displayer is cumulative;
        /// the sum of amounts is used if the displayer is non-cumulative.
        /// </summary>
        /// <param name="cost">The cost to evaluate the display amound for</param>
        public int GetDisplayAmount(Cost cost)
        {
            Cost baseCost = cost.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                // for multicosts, we need to account for any cost which might be nested there.
                // this is because any wrapper cost may not produce the appropriate information
                // needed to generate a display amount.
                IEnumerable<Cost> validCosts = mc.Select(cc => cc.GetBaseCost()).Where(SupportsCost);
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

        /// <summary>
        /// Gets a text representation of costs which are not included in the display amount.
        /// </summary>
        /// <param name="cost">The cost to evaluate cost text for</param>
        public string GetAdditionalCostText(Cost cost)
        {
            // we always check if the base cost is supported to account for wrappers, but get the text off the
            // top-level cost even if it is a wrapper. This allows wrapper costs to implement changes to GetCostText.
            Cost baseCost = cost.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                return string.Join(Language.Language.Get("COMMA_SPACE", "IC"), mc.Where(c => !SupportsCost(c.GetBaseCost()))
                    .Select(c => c.GetCostText()).ToArray());
            }
            else if (!SupportsCost(baseCost))
            {
                return cost.GetCostText();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Whether this displayer can support a given cost.
        /// </summary>
        /// <remarks>
        /// Implementers may assume that any wrapper costs have been removed (i.e. directly type checking 
        /// a given cost without calling GetBaseCost is acceptable).
        /// </remarks>
        /// <param name="cost">The cost to evaluate support for</param>
        protected abstract bool SupportsCost(Cost cost);

        /// <summary>
        /// Gets the display amount for a single cost in the displayer.
        /// </summary>
        /// <remarks>
        /// Implementers may assume that the cost has already been checked and confirmed to be supported
        /// (i.e. direct type casting without checking first is acceptable).
        /// </remarks>
        /// <param name="cost"></param>
        /// <returns></returns>
        protected abstract int GetSingleCostDisplayAmount(Cost cost);
    }

    /// <summary>
    /// A noncumulative cost displayer which displays geo costs (as in vanilla shops).
    /// </summary>
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

    /// <summary>
    /// A cost displayer which displays PD int costs matching a provided PD variable name. When set
    /// set to cumulative, it will check for PDIntCost; when set to noncumulative it will check for
    /// ConsumablePDIntCost. Cost sprite is user-customizable and defaults to null.
    /// </summary>
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

    /// <summary>
    /// A cumulative cost displayer which displays rancid egg costs with a default cost sprite
    /// of a rancid egg
    /// </summary>
    public class EggCostDisplayer : CostDisplayer
    {
        public override ISprite CustomCostSprite { get; set; } = new ItemChangerSprite("ShopIcons.RancidEgg");

        public override bool Cumulative => true;

        protected override bool SupportsCost(Cost cost) => cost is CumulativeRancidEggCost;

        protected override int GetSingleCostDisplayAmount(Cost cost) => (cost as CumulativeRancidEggCost).Total;
    }
}
