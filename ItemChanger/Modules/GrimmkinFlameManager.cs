using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using Modding;
using Newtonsoft.Json;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which overrides the PlayerData.flamesCollected field to ensure having more than 3 flames is compatible with the Grimm quest.
    /// </summary>
    [DefaultModule]
    public class GrimmkinFlameManager : Module
    {
        public int cumulativeFlamesSpent { get; set; }

        [JsonIgnore] public int cumulativeFlamesCollected => PlayerData.instance.GetIntInternal(nameof(PlayerData.flamesCollected));
        [JsonIgnore] public int flameBalance => cumulativeFlamesCollected - cumulativeFlamesSpent;
        [JsonIgnore] public int cappedFlameBalance => Math.Min(3, flameBalance);

        public override void Initialize()
        {
            ModHooks.GetPlayerIntHook += OverrideGetFlamesCollected;
            On.PlayerData.SetInt += OverrideSetFlamesCollected;
            Events.AddFsmEdit(SceneNames.Grimm_Main_Tent, new("Grimm Scene", "Initial Scene"), EditGrimmInitial);
            Events.AddFsmEdit(SceneNames.Grimm_Main_Tent, new("Defeated NPC", "Conversation Control"), EditGrimmDefeated);
        }

        public override void Unload()
        {
            ModHooks.GetPlayerIntHook -= OverrideGetFlamesCollected;
            On.PlayerData.SetInt -= OverrideSetFlamesCollected;
            Events.RemoveFsmEdit(SceneNames.Grimm_Main_Tent, new("Grimm Scene", "Initial Scene"), EditGrimmInitial);
            Events.RemoveFsmEdit(SceneNames.Grimm_Main_Tent, new("Defeated NPC", "Conversation Control"), EditGrimmDefeated);
        }

        // We use the On hook to route flame decreases by any cause into cumulativeFlamesSpent.
        // This is mostly redundant, since all vanilla flame decreases are rerouted through the fsm hook.
        private void OverrideSetFlamesCollected(On.PlayerData.orig_SetInt orig, PlayerData pd, string intName, int newValue)
        {
            switch (intName)
            {
                case nameof(PlayerData.flamesCollected):
                    int diff = newValue - cumulativeFlamesCollected;
                    if (diff < 0)
                    {
                        orig(pd, nameof(cumulativeFlamesSpent), cumulativeFlamesSpent - diff);
                    }
                    else
                    {
                        orig(pd, intName, newValue);
                    }
                    return;
            }
            orig(pd, intName, newValue);
        }

        // Grimm only appears in his tent if the player has exactly 3 flames. Hide any excess
        // flames (which can only happen when flames are randomized) from the game.
        // Increments of the variable (collecting flames) will still increment the real value.
        private int OverrideGetFlamesCollected(string name, int orig) => name switch
        {
            nameof(PlayerData.flamesCollected) => cappedFlameBalance,
            nameof(cumulativeFlamesCollected) => cumulativeFlamesCollected,
            nameof(cumulativeFlamesSpent) => cumulativeFlamesSpent,
            nameof(flameBalance) => flameBalance,
            _ => orig,
        };

        // We edit the fsms of each Grimmchild upgrade which would otherwise set flame count to 0 instead of decrementing by 3.

        private void EditGrimmInitial(PlayMakerFSM fsm)
        {
            FsmState levelUpTo2 = fsm.GetState("Level Up To 2");

            int i = Array.FindIndex(levelUpTo2.Actions, a => a is SetPlayerDataInt spdi && spdi.intName.Value == "flamesCollected");
            if (i >= 0)
            {
                levelUpTo2.Actions[i] = new Lambda(PayGrimmUpgrade);
            }
        }

        private void EditGrimmDefeated(PlayMakerFSM fsm)
        {
            FsmState levelUpTo3 = fsm.GetState("Level Up To 3");

            int i = Array.FindIndex(levelUpTo3.Actions, a => a is SetPlayerDataInt spdi && spdi.intName.Value == "flamesCollected");
            if (i >= 0)
            {
                levelUpTo3.Actions[i] = new Lambda(PayGrimmUpgrade);
            }
        }

        private void PayGrimmUpgrade()
        {
            cumulativeFlamesSpent += PlayerData.instance.GetInt(nameof(PlayerData.flamesRequired));
        }
    }


    /// <summary>
    /// A Grimmkin Flame cost with options to act cumulatively.
    /// </summary>
    /// <param name="amount">The number of flames.</param>
    /// <param name="cumulative">Should the cost compare against the cumulative flames collected, or the current balance?</param>
    /// <param name="subtractive">Should paying the cost increment the cumulative flames spent, and decrease the current balance?</param>
    public record FlameCost(int amount, bool cumulative, bool subtractive) : Cost
    {
        public override bool CanPay()
        {
            return cumulative ? 
                ItemChangerMod.Modules.GetOrAdd<GrimmkinFlameManager>().cumulativeFlamesCollected >= amount : 
                ItemChangerMod.Modules.GetOrAdd<GrimmkinFlameManager>().flameBalance >= amount;
        }

        public override string GetCostText()
        {
            int balance = cumulative ? amount - ItemChangerMod.Modules.GetOrAdd<GrimmkinFlameManager>().cumulativeFlamesSpent : amount;
            if (balance <= 0) return "Free";
            else return subtractive ? $"Pay {amount} Grimmkin Flames" : $"Requires {amount} Grimmkin Flames";
        }

        public override bool HasPayEffects()
        {
            return subtractive;
        }

        public override void OnPay()
        {
            if (subtractive)
            {
                var gfm = ItemChangerMod.Modules.GetOrAdd<GrimmkinFlameManager>();
                int balance = cumulative ? amount - gfm.cumulativeFlamesSpent : amount;
                if (balance > 0) gfm.cumulativeFlamesSpent += balance;
            }
        }
    }
}
