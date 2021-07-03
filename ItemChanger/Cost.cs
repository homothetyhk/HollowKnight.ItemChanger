using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public abstract class Cost
    {
        public abstract bool CanPay();
        public void Pay()
        {
            OnPay();
            Paid = true;
        }

        public virtual void OnPay() { }
        public bool Paid { get; protected set; }

        public abstract string GetCostText();

        /// <summary>
        /// Controls the number displayed in shops, etc.
        /// </summary>
        public virtual int GetGeoValue()
        {
            return 0;
        }

        public static Cost operator +(Cost a, Cost b)
        {
            MultiCost aa = a as MultiCost;
            MultiCost bb = b as MultiCost;
            
            if (aa != null && bb != null)
            {
                return new MultiCost { costs = aa.costs.Concat(bb.costs).ToList() };
            }

            if (aa != null)
            {
                var l = aa.costs.ToList();
                l.Add(b);
                return new MultiCost { costs = l };
            }

            if (bb != null)
            {
                var l = bb.costs.ToList();
                l.Add(a);
                return new MultiCost { costs = l };
            }

            return new MultiCost
            {
                costs = new List<Cost> { a, b }
            };
        }

        public static Cost NewGeoCost(int amount)
        {
            return new GeoCost
            {
                amount = amount,
            };
        }

        public static Cost NewEssenceCost(int amount)
        {
            return new PDIntCost
            {
                amount = amount,
                fieldName = nameof(PlayerData.dreamOrbs),
                uiText = $"Requires {amount} Essence",
            };
        }

        public static Cost NewGrubCost(int amount)
        {
            return new PDIntCost
            {
                amount = amount,
                fieldName = nameof(PlayerData.grubsCollected),
                uiText = $"Requires {amount} Grub{(amount == 1 ? string.Empty : "s")}",
            };
        }
    }

    public class MultiCost : Cost
    {
        public List<Cost> costs;

        public override bool CanPay()
        {
            return costs.All(c => c.Paid || c.CanPay());
        }

        public override void OnPay()
        {
            foreach (Cost c in costs)
            {
                if (!c.Paid) c.Pay();
            }
        }

        public override int GetGeoValue()
        {
            return costs.Sum(c => c.GetGeoValue());
        }

        public override string GetCostText()
        {
            return string.Join(", ", costs.Select(c => c.GetCostText()).ToArray());
        }
    }


    public class PDBoolCost : Cost
    {
        public string fieldName;
        public string uiText;

        public override bool CanPay() => PlayerData.instance.GetBool(fieldName);

        public override string GetCostText()
        {
            return uiText;
        }
    }

    public class PDIntCost : Cost
    {
        public string fieldName;
        public string uiText;
        public int amount;
        public ComparisonOperator op = ComparisonOperator.Ge;

        public override bool CanPay() => PlayerData.instance.GetInt(fieldName).Compare(op, amount);

        public override string GetCostText()
        {
            return uiText;
        }
    }

    public class ConsumablePDIntCost : Cost
    {
        public string fieldName;
        public string uiText;
        public int amount;

        public override bool CanPay() => PlayerData.instance.GetInt(fieldName) >= amount;
        public override void OnPay()
        {
            PlayerData.instance.IntAdd(fieldName, -amount);
        }

        public override string GetCostText()
        {
            return uiText;
        }
    }

    public class GeoCost : Cost
    {
        public int amount;
        public override bool CanPay() => PlayerData.instance.GetInt(nameof(PlayerData.geo)) >= amount;
        public override void OnPay()
        {
            HeroController.instance.TakeGeo(amount);
        }

        public override int GetGeoValue()
        {
            return amount;
        }

        public override string GetCostText()
        {
            return $"Pay {amount} geo";
        }


    }

}
