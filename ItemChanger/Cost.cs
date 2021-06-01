using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public abstract class Cost
    {
        public bool paid;

        public abstract bool CanPay();
        public void Pay()
        {
            OnPay();
            paid = true;
        }

        public virtual void OnPay() { }
        public bool Paid() => paid;

        public abstract string GetCostText();




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

        public override bool CanPay() => PlayerData.instance.GetInt(fieldName) >= amount;

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

        public override string GetCostText()
        {
            return $"Pay {amount} geo";
        }


    }

}
