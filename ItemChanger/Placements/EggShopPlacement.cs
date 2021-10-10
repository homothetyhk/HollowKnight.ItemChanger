using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Locations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class EggShopPlacement : AbstractPlacement, IMultiCostPlacement
    {
        public EggShopPlacement(string Name) : base(Name) { }

        public Dictionary<int, string> containers = new();

        public PlaceableLocation Location;

        [Newtonsoft.Json.JsonProperty]
        public bool PurchasedAll { get; private set; }

        protected override void OnLoad()
        {
            Events.AddSceneChangeEdit(Location.sceneName, OnActiveSceneChanged);
            Events.AddFsmEdit(SceneNames.Room_Ouiji, new("Jiji NPC", "Conversation Control"), JijiConvoEdit);
            Events.AddLanguageEdit(new("Jiji", "GREET"), JijiGreet);
            Events.AddLanguageEdit(new("Jiji", "SHADE_OFFER"), JijiShadeOffer);
            Events.AddLanguageEdit(new("Jiji", "DECLINE"), JijiDecline);
            Events.AddLanguageEdit(new("Prompts", "JIJI_OFFER"), JijiYNOffer);
        }

        protected override void OnUnload()
        {
            Events.RemoveSceneChangeEdit(Location.sceneName, OnActiveSceneChanged);
            Events.RemoveFsmEdit(SceneNames.Room_Ouiji, new("Jiji NPC", "Conversation Control"), JijiConvoEdit);
            Events.RemoveLanguageEdit(new("Jiji", "GREET"), JijiGreet);
            Events.RemoveLanguageEdit(new("Jiji", "SHADE_OFFER"), JijiShadeOffer);
            Events.RemoveLanguageEdit(new("Jiji", "DECLINE"), JijiDecline);
            Events.RemoveLanguageEdit(new("Prompts", "JIJI_OFFER"), JijiYNOffer);
        }

        public bool CanPurchaseAny() => Items.Any(i => i.GetTag<CostTag>()?.Cost is Cost c && !c.Paid && c.CanPay());

        public void PurchaseAndSpawnNewItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (!Items[i].IsObtained() && Items[i].GetTag<CostTag>()?.Cost is Cost c && !c.Paid && c.CanPay())
                {
                    c.Pay();
                    SpawnItem(i);
                }
            }
        }

        public void SpawnItem(int i)
        {
            if (!containers.TryGetValue(i, out string container))
            {
                container = Items[i].GetPreferredContainer();
                if (string.IsNullOrEmpty(container) || container == Container.Unknown) container = Container.Shiny;
                containers[i] = container;
            }
            Container c = Container.GetContainer(container);
            if (c == null || !c.SupportsInstantiate)
            {
                containers[i] = Container.Shiny;
                c = Container.GetContainer(Container.Shiny);
            }

            GameObject obj = c.GetNewContainer(this, Items[i].Yield(), Location.flingType);
            Location.PlaceContainer(obj, container);
            obj.transform.Translate(new((i % 5) - 2f, 0));
            obj.SetActive(true);
        }

        private void OnActiveSceneChanged(Scene to)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (!Items[i].IsObtained() && (Items[i].GetTag<CostTag>()?.Cost?.Paid ?? true))
                {
                    SpawnItem(i);
                }
            }

            PurchasedAll = Items.All(i => i.GetTag<CostTag>()?.Cost?.Paid ?? true);
        }

        private void JijiConvoEdit(PlayMakerFSM jijiFsm)
        {
            if (PurchasedAll) return;

            // Set the "Black Wave" to white
            Transform bw = jijiFsm.transform.Find("Black Wave");
            bw.GetComponent<WaveEffectControl>().blackWave = false;
            bw.GetComponent<SpriteRenderer>().color = Color.white;

            FsmState convoChoice = jijiFsm.GetState("Convo Choice");
            FsmState greet = jijiFsm.GetState("Greet");
            FsmState offer = jijiFsm.GetState("Offer");
            FsmState haveEggs = jijiFsm.GetState("Have Eggs?");
            FsmState yes = jijiFsm.GetState("Yes");
            FsmState spawn = jijiFsm.GetState("Spawn");

            convoChoice.Actions = new FsmStateAction[]
            {
                    convoChoice.Actions[1], // AudioPlayerOneShot, all other actions trigger alternate conversations
            };
            convoChoice.AddTransition(FsmEvent.Finished, greet); // Always display the Jiji:GREET convo

            greet.ClearTransitions();
            greet.AddTransition("CONVO_FINISH", offer); // Always display the Jiji:SHADE_OFFER convo

            // replace IntCompare for rancid eggs with test based on item costs
            haveEggs.Actions[0] = new BoolTestMod(CanPurchaseAny, "YES", "NO");

            // remove shade info edits
            yes.RemoveActionsOfType<SetPlayerDataString>();
            yes.RemoveActionsOfType<SetPlayerDataInt>();
            yes.RemoveActionsOfType<SetPlayerDataFloat>();
            yes.RemoveActionsOfType<PlayerDataIntAdd>();

            // Jiji:RITUAL_BEGIN

            spawn.RemoveActionsOfType<CreateObject>();
            spawn.AddLastAction(new Lambda(() =>
            {
                PurchaseAndSpawnNewItems();
            }));
        }

        private void JijiGreet(ref string value)
        {
            if (!PurchasedAll)
            {
                value = "Ah, hello. How have you been faring? Have you come to bring me something tasty? Let me see...";
            }
        }
        private void JijiShadeOffer(ref string value)
        {
            if (!PurchasedAll)
            {
                StringBuilder sb = new();
                for (int i = 0; i < Items.Count; i++)
                {
                    if (!Items[i].IsObtained() && Items[i].GetTag<CostTag>()?.Cost is Cost c && !c.Paid)
                    {
                        if (i + 1 < Items.Count) sb.AppendLine($"{c.GetCostText()}  -  {Items[i].GetResolvedUIDef(this).GetPreviewName()}");
                        else sb.Append($"{c.GetCostText()}  -  {Items[i].GetResolvedUIDef(this).GetPreviewName()}");
                    }
                }
                value = sb.ToString();
            }
        }
        private void JijiDecline(ref string value)
        {
            if (!PurchasedAll)
            {
                value = "Oh? Well, if you have no desire to get my items, I can not help you.";
            }
        }
        private void JijiYNOffer(ref string value)
        {
            if (!PurchasedAll)
            {
                value = "Give Jiji all that you can?";
            }
        }
    }
}
