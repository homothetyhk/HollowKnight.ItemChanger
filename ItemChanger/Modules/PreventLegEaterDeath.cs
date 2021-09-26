using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class PreventLegEaterDeath : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Fungus2_26, new("Leg Eater", "Conversation Control"), RemoveConversationOptions);
            Modding.ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Fungus2_26, new("Leg Eater", "Conversation Control"), RemoveConversationOptions);
            Modding.ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
        }

        private bool GetPlayerBoolHook(string name, bool orig)
        {
            if (name == nameof(PlayerData.legEaterLeft)) return false;
            else return orig;
        }

        private void RemoveConversationOptions(PlayMakerFSM fsm)
        {
            FsmState legEaterChoice = fsm.GetState("Convo Choice");
            legEaterChoice.RemoveTransitionsTo("Convo 1");
            legEaterChoice.RemoveTransitionsTo("Convo 2");
            legEaterChoice.RemoveTransitionsTo("Convo 3");
            legEaterChoice.RemoveTransitionsTo("Infected Crossroad");
            legEaterChoice.RemoveTransitionsTo("Bought Charm");
            legEaterChoice.RemoveTransitionsTo("Gold Convo");
            legEaterChoice.RemoveTransitionsTo("All Gold");
            legEaterChoice.RemoveTransitionsTo("Ready To Leave");
            FsmState allGold = fsm.GetState("All Gold?");
            allGold.RemoveTransitionsTo("No Shop");
        }
    }
}
