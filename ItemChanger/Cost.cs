using ItemChanger.Extensions;
using Newtonsoft.Json;

namespace ItemChanger
{
    public abstract record Cost
    {
        public abstract bool CanPay();
        public void Pay()
        {
            OnPay();
            Paid = true;
        }

        public abstract void OnPay();

        [JsonProperty]
        public bool Paid { get; protected set; }

        public abstract string GetCostText();
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
            return new PDIntCost(amount, nameof(PlayerData.dreamOrbs), $"Requires {amount} Essence");
        }

        public static Cost NewGrubCost(int amount)
        {
            return new PDIntCost(amount, nameof(PlayerData.grubsCollected), $"Requires {amount} Grub{(amount == 1 ? string.Empty : "s")}");
        }
    }

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
            return string.Join(", ", Costs.Select(c => c.GetCostText()).ToArray());
        }

        public override string GetShopCostText()
        {
            return string.Join(", ", Costs.Where(c => !(c is GeoCost))
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

    }


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
            return uiText;
        }
    }

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
            return uiText;
        }
    }

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
            return uiText;
        }
    }

    public record GeoCost(int amount) : Cost
    {
        public override bool CanPay() => PlayerData.instance.GetInt(nameof(PlayerData.geo)) >= amount;
        public override void OnPay()
        {
            HeroController.instance.TakeGeo(amount);
        }

        public override bool HasPayEffects() => true;

        public override bool Includes(Cost c)
        {
            if (c is GeoCost gc) return gc.amount <= amount;
            return base.Includes(c);
        }

        public override int GetDisplayGeo()
        {
            return amount;
        }

        public override string GetCostText()
        {
            return $"Pay {amount} geo";
        }

        public override string GetShopCostText()
        {
            return null;
        }
    }
}
