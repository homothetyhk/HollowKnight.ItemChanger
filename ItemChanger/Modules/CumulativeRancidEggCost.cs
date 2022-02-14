using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Subtractive rancid egg cost which adjusts for the number of eggs previously spent on this type of cost.
    /// </summary>
    public record CumulativeRancidEggCost(int Total) : Cost
    {
        public override void Load()
        {
            module = ItemChangerMod.Modules.GetOrAdd<CumulativeEggCostModule>();
            module.MaximumCost = Math.Max(Total, module.MaximumCost);
        }

        private CumulativeEggCostModule module;

        public int GetBalance()
        {
            if (module == null) Load();
            return Total - module.CumulativeEggsSpent;
        }

        public override bool CanPay() => GetBalance() <= PlayerData.instance.GetInt(nameof(PlayerData.rancidEggs));

        public override void OnPay()
        {
            int bal = GetBalance();
            if (bal > 0)
            {
                PlayerData.instance.IntAdd(nameof(PlayerData.rancidEggs), -bal);
                module.CumulativeEggsSpent += bal;
            }
        }

        public override bool HasPayEffects() => true;
        public override bool Includes(Cost c)
        {
            if (c is CumulativeRancidEggCost rec) return rec.Total <= Total;
            else return base.Includes(c);
        }

        public override string GetCostText()
        {
            int bal = GetBalance();
            if (bal == 1) return Language.Language.Get("PAY_EGG", "IC");
            else if (bal > 0) return string.Format(Language.Language.Get("PAY_EGGS", "Fmt"), bal);
            else return Language.Language.Get("FREE", "IC");
        }
    }

    /// <summary>
    /// Module which tracks the number of rancid eggs that have been spent via CumulativeRancidEggCost.
    /// </summary>
    [DefaultModule]
    public class CumulativeEggCostModule : Module
    {
        public int CumulativeEggsSpent;
        public int MaximumCost;
        public bool DeactivateJinnUntilMaximumCostSpent = true;

        public override void Initialize() 
        {
            Events.AddFsmEdit(SceneNames.Room_Jinn, new("Jinn NPC", "Wake and Animate"), EditJinn);
        }

        public override void Unload() 
        {
            Events.RemoveFsmEdit(SceneNames.Room_Jinn, new("Jinn NPC", "Wake and Animate"), EditJinn);
        }

        private void EditJinn(PlayMakerFSM fsm)
        {
            if (!DeactivateJinnUntilMaximumCostSpent || MaximumCost <= CumulativeEggsSpent) return;

            FsmState sleep = fsm.GetState("Sleep");
            sleep.ClearTransitions();
        }
    }
}
