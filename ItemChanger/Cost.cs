using ItemChanger.Extensions;
using Newtonsoft.Json;

namespace ItemChanger
{
    /// <summary>
    /// Data type used generally for cost handling, including in shops and y/n dialogue prompts.
    /// </summary>
    public abstract record Cost
    {
        /// <summary>
        /// Returns whether the cost can currently be paid.
        /// </summary>
        public abstract bool CanPay();

        /// <summary>
        /// Pays the cost, performing any effects and setting the cost to Paid.
        /// </summary>
        public virtual void Pay()
        {
            OnPay();
            Paid = true;
        }

        /// <summary>
        /// Method for administering all effects of the cost during Pay.
        /// </summary>
        public abstract void OnPay();

        /// <summary>
        /// Represents whether the cost has been paid yet. Paid costs will be subsequently ignored.
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// A number between 0 and 1 which modifies numeric costs. Only considered by some costs.
        /// <br/>For example, the Leg Eater dung discount sets this to 0.8, to indicate that geo costs should be at 80% price.
        /// </summary>
        public virtual float DiscountRate { get; set; } = 1.0f;

        /// <summary>
        /// Method which provides the cost text used in y/n prompts.
        /// </summary>
        public abstract string GetCostText();

        /// <summary>
        /// Method which provides the description of the cost displayed below the item description in the shop window.
        /// </summary>
        public virtual string GetShopCostText()
        {
            return GetCostText();
        }

        /// <summary>
        /// Controls the number displayed in shops, etc.
        /// </summary>
        public virtual int GetDisplayGeo()
        {
            return 0;
        }

        /// <summary>
        /// Is the other cost a subset of this cost?
        /// </summary>
        public virtual bool Includes(Cost c) =>  c is null || Equals(c);

        /// <summary>
        /// Does paying this cost have effects (particularly that could prevent paying other costs of the same type)?
        /// </summary>
        public abstract bool HasPayEffects();
        /// <summary>
        /// Method which should be called by the Cost's owner during initial loading. Used by certain costs which require global or shared tracking.
        /// </summary>
        public virtual void Load() { }
        /// <summary>
        /// Method which should be called by the Cost's owner during unloading. Used by certain costs which require global or shared tracking.
        /// </summary>
        public virtual void Unload() { }

        /// <summary>
        /// Combines two costs into a MultiCost. If either argument is null, returns the other argument.  If one or both costs is a MultiCost, flattens the result.
        /// </summary>
        public static Cost operator +(Cost a, Cost b)
        {
            if (a == null) return b;
            if (b == null) return a;

            MultiCost aa = a as MultiCost;
            MultiCost bb = b as MultiCost;
            
            if (aa != null && bb != null)
            {
                return new MultiCost(aa.Costs.Concat(bb.Costs));
            }

            if (aa != null)
            {
                var l = aa.Costs.ToList();
                l.Add(b);
                return new MultiCost(l);
            }

            if (bb != null)
            {
                var l = bb.Costs.ToList();
                l.Add(a);
                return new MultiCost(l);
            }

            return new MultiCost(a, b);
        }

        public static Cost NewGeoCost(int amount)
        {
            return new GeoCost(amount);
        }

        public static Cost NewEssenceCost(int amount)
        {
            return new PDIntCost(amount, nameof(PlayerData.dreamOrbs), string.Format(Language.Language.Get("REQUIRES_ESSENCE", "Fmt"), amount));
        }

        public static Cost NewGrubCost(int amount)
        {
            return new PDIntCost(amount, nameof(PlayerData.grubsCollected), amount == 1 ? Language.Language.Get("REQUIRES_GRUB", "IC")
                : string.Format(Language.Language.Get("REQUIRES_GRUBS", "Fmt"), amount));
        }
    }

    /// <summary>
    /// Cost which is the concatenation of other costs. Can only be paid if all of its costs can be paid, and pays all its costs sequentially.
    /// </summary>
    public record MultiCost : Cost
    {
        public MultiCost()
        {
            Costs = new();
        }

        [JsonConstructor]
        public MultiCost(List<Cost> Costs)
        {
            this.Costs = Costs;
        }

        public MultiCost(IEnumerable<Cost> Costs)
        {
            this.Costs = new(Costs);
        }

        public MultiCost(params Cost[] Costs)
        {
            this.Costs = new(Costs);
        }

        public List<Cost> Costs { get; }



        public override bool CanPay()
        {
            return Costs.All(c => c.Paid || c.CanPay());
        }

        public override void OnPay()
        {
            foreach (Cost c in Costs)
            {
                if (!c.Paid) c.Pay();
            }
        }

        public override int GetDisplayGeo()
        {
            return Costs.Sum(c => c.GetDisplayGeo());
        }

        public override string GetCostText()
        {
            return string.Join(Language.Language.Get("COMMA_SPACE", "IC"), Costs.Select(c => c.GetCostText()).ToArray());
        }

        public override string GetShopCostText()
        {
            return string.Join(Language.Language.Get("COMMA_SPACE", "IC"), Costs.Where(c => !(c is GeoCost))
                .Select(c => c.GetCostText()).ToArray());
        }

        public override bool Includes(Cost c)
        {
            if (c is MultiCost mc)
            {
                return mc.Costs.All(d => Includes(d));
            }

            return Costs.Any(d => d.Includes(c));
        }

        public override bool HasPayEffects()
        {
            return Costs.Any(d => d.HasPayEffects());
        }

        public override void Load()
        {
            foreach (Cost c in Costs) c.Load();
        }

        public override void Unload()
        {
            foreach (Cost c in Costs) c.Unload();
        }
    }

    /// <summary>
    /// Cost which has no pay effects, but can only be paid when the specified PlayerData bool is true.
    /// </summary>
    public record PDBoolCost(string fieldName, string uiText) : Cost
    {
        public override bool CanPay() => PlayerData.instance.GetBool(fieldName);
        public override void OnPay() { }
        public override bool HasPayEffects() => false;
        public override bool Includes(Cost c)
        {
            if (c is PDBoolCost pbc) return pbc.fieldName == fieldName;
            else return base.Includes(c);
        }
        public override string GetCostText()
        {
            return Language.Language.Get(uiText, "Exact");
        }
    }

    /// <summary>
    /// Cost which has no pay effects, but can only be paid when the specified PlayerData int comparison succeeds.
    /// </summary>
    public record PDIntCost(int amount, string fieldName, string uiText, ComparisonOperator op = ComparisonOperator.Ge) : Cost
    {
        public override bool CanPay() => PlayerData.instance.GetInt(fieldName).Compare(op, amount);
        public override void OnPay() { }
        public override bool HasPayEffects() => false;
        public override bool Includes(Cost c)
        {
            if (c is PDIntCost pic) return pic.amount <= amount && pic.op == op && pic.fieldName == fieldName;
            return base.Includes(c);
        }
        public override string GetCostText()
        {
            return Language.Language.Get(uiText, "Exact");
        }
    }

    /// <summary>
    /// Cost which subtracts the specified amount from the specified PlayerData int. Can only be paid when the result of the subtraction would be nonnegative.
    /// </summary>
    public record ConsumablePDIntCost(int amount, string fieldName, string uiText) : Cost
    {
        public override bool CanPay() => PlayerData.instance.GetInt(fieldName) >= amount;
        public override void OnPay()
        {
            PlayerData.instance.IntAdd(fieldName, -amount);
        }

        public override bool HasPayEffects() => true;
        public override bool Includes(Cost c)
        {
            if (c is ConsumablePDIntCost pic) return pic.amount <= amount && pic.fieldName == fieldName;
            return base.Includes(c);
        }

        public override string GetCostText()
        {
            return Language.Language.Get(uiText, "Exact");
        }
    }

    /// <summary>
    /// Cost which subtracts the specified amount from the GeoCounter. Can only be paid when the result of the subtraction would be nonnegative.
    /// </summary>
    public record GeoCost(int amount) : Cost
    {
        public override bool CanPay() => PlayerData.instance.GetInt(nameof(PlayerData.geo)) >= (int)(amount * DiscountRate);
        public override void OnPay()
        {
            int amt = (int)(amount * DiscountRate);
            if (amt > 0) HeroController.instance.TakeGeo(amt);
        }

        public override bool HasPayEffects() => true;

        public override bool Includes(Cost c)
        {
            if (c is GeoCost gc) return gc.amount <= amount;
            return base.Includes(c);
        }

        public override int GetDisplayGeo()
        {
            return (int)(amount * DiscountRate);
        }

        public override string GetCostText()
        {
            return string.Format(Language.Language.Get("PAY_GEO", "Fmt"), (int)(amount * DiscountRate));
        }

        public override string GetShopCostText()
        {
            return null;
        }
    }
}
