using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Locations.SpecialLocations
{
    public class DivineLocation : ExistingFsmContainerLocation
    {
        public enum DivineShopSlot
        {
            Heart,
            Greed,
            Strength,
        }

        public DivineShopSlot shopSlot;
        public int requiredCharmID;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(SceneNames.Grimm_Divine, new("Divine NPC", "Conversation Control"), EditDivineFsm);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(SceneNames.Grimm_Divine, new("Divine NPC", "Conversation Control"), EditDivineFsm);
        }

        public override bool HandlesCostBeforeContainer => true;

        protected override void OnReplace(GameObject obj, Container c)
        {
            ObjectLocation.FindGameObject("Divine NPC").LocateFSM("Conversation Control").AddFsmGameObject("Replace Object " + shopSlot.ToString(), obj);
        }

        private bool ShouldGiveItem()
        {
            return PlayerData.instance.GetBool("equippedCharm_" + requiredCharmID) && !Placement.AllObtained();
        }

        private bool ItemIsPooed()
        {
            return PlayerData.instance.GetBool("pooedFragile" + shopSlot.ToString()) && !Placement.AllObtained();
        }

        private Cost GetCost()
        {
            if (Placement is Placements.ISingleCostPlacement iscp && iscp.Cost is Cost c) return c;
            return Placement.GetTag<CostTag>()?.Cost;
        }

        private bool IsOnThisSlot(PlayMakerFSM fsm)
        {
            return fsm.FsmVariables.FindFsmInt("Current Charm").Value == 1 + (int)shopSlot;
        }

        private void EditSlotSpecificPath(PlayMakerFSM fsm)
        {
            fsm.AddFsmInt($"Required Charm {shopSlot}", requiredCharmID); // fsmvar used in building Greet text

            FsmState pooed = fsm.GetState($"Pooed {shopSlot}?");
            pooed.Actions[0] = new DelegateBoolTest(ItemIsPooed, FsmEvent.GetFsmEvent("SPAWN"), FsmEvent.Finished);

            FsmState spawnReady = fsm.GetState("Spawn Ready");
            spawnReady.Actions[1 + (int)shopSlot] = new Lambda(() => ActivateIfModdedPooed(fsm));

            FsmState choice = fsm.GetState("Choice");
            string gaveBool = "gaveFragile" + shopSlot.ToString();
            int i = Array.FindIndex(choice.Actions, a => a is PlayerDataBoolTest pdbt && pdbt.boolName.Value == gaveBool);
            choice.RemoveAction(i);
            string equipEvent = "EQUIPPED " + (shopSlot == DivineShopSlot.Greed ? "GEO" : shopSlot.ToString().ToUpper());
            i = Array.FindIndex(choice.Actions, a => a is PlayerDataBoolTrueAndFalse pdbtaf && pdbtaf.isTrue?.Name == equipEvent);
            choice.Actions[i] = new DelegateBoolTest(ShouldGiveItem, FsmEvent.GetFsmEvent(equipEvent), null);

            FsmState request = fsm.GetState("Mod Request");
            request.AddFirstAction(new Lambda(() =>
            {
                if (IsOnThisSlot(fsm))
                {
                    DialogueCenter.StartConversation($"Aaaaaaaaahhhhhhhh!<page>That {CharmNameUtil.GetCharmName(requiredCharmID)}... beautiful! " +
                        $"Most precious thing! Little lovely, will you let me see it? Have to show it to me! ");
                }
            }));

            FsmState afterChoice = new(fsm.Fsm)
            {
                Name = "Mod Request " + shopSlot.ToString(),
            };
            afterChoice.Actions = new FsmStateAction[]
            {
                new Lambda(() => fsm.FsmVariables.FindFsmInt("Current Charm").Value = 1 + (int)shopSlot),
            };
            choice.Transitions.First(t => t.EventName == equipEvent).SetToState(afterChoice);
            afterChoice.AddTransition(FsmEvent.Finished, request);

            FsmState sendText = fsm.GetState("Mod Send Text");
            sendText.AddFirstAction(new Lambda(() =>
            {
                if (IsOnThisSlot(fsm))
                {
                    YNUtil.OpenYNDialogue(fsm.gameObject, Placement, Placement.Items, GetCost());
                }
            }));

            FsmState yes = fsm.GetState("Mod Yes");
            yes.AddFirstAction(new Lambda(() =>
            {
                if (IsOnThisSlot(fsm))
                {
                    Cost c = GetCost();
                    if (c is not null && !c.Paid) c.Pay();
                    fsm.FsmVariables.FindFsmString("Pooed PD Bool").Value = "pooedFragile" + shopSlot.ToString();
                    fsm.FsmVariables.FindFsmGameObject("Charm To Spawn").Value = FindContainer(fsm, shopSlot);
                }
            }));
        }

        private static void Activate(PlayMakerFSM fsm, DivineShopSlot slot)
        {
            FindContainer(fsm, slot).SetActive(true);
        }

        private static void ActivateOnEnd(PlayMakerFSM fsm)
        {
            GameObject obj = fsm.FsmVariables.FindFsmGameObject("Charm To Spawn").Value;
            if (obj.LocateFSM("Shiny Control") is PlayMakerFSM shinyFsm) ShinyUtility.FlingShinyLeft(shinyFsm);
            obj.SetActive(true);
        }

        private static GameObject FindContainer(PlayMakerFSM fsm, DivineShopSlot slot)
        {
            if (fsm.FsmVariables.FindFsmGameObject("Replace Object " + slot.ToString()) is FsmGameObject fgo)
            {
                return fgo.Value;
            }
            else
            {
                return fsm.FsmVariables.FindFsmGameObject("Charm Holder").Value.FindChild("Poo " + slot.ToString());
            }
        }

        private static void ActivateShinyIfVanillaPooed(PlayMakerFSM fsm, DivineShopSlot slot)
        {
            string unbreakableBool = slot switch
            {
                DivineShopSlot.Heart => nameof(PlayerData.fragileHealth_unbreakable),
                DivineShopSlot.Greed => nameof(PlayerData.fragileGreed_unbreakable),
                _ => nameof(PlayerData.fragileStrength_unbreakable),
            };
            if (PlayerData.instance.GetBool("pooedFragile" + slot.ToString()) && !PlayerData.instance.GetBool(unbreakableBool))
            {
                Activate(fsm, slot);
            }
        }

        private void ActivateIfModdedPooed(PlayMakerFSM fsm)
        {
            if (ItemIsPooed()) Activate(fsm, shopSlot);
        }

        private void CreateModdedPath(PlayMakerFSM fsm)
        {
            FsmState spawnReady = fsm.GetState("Spawn Ready");
            spawnReady.Actions = new FsmStateAction[]
            {
                spawnReady.Actions[0], // SetCollider false
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Heart)), // actions to be replaced by the individual placements
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Greed)),
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Strength)),
                new Wait
                {
                    time = 4f,
                    realTime = false,
                    finishEvent = spawnReady.Transitions[0].FsmEvent,
                },
            };
            foreach (string s in new[] { "Spawn Heart", "Spawn Greed", "Spawn Strength" }) fsm.GetState(s).ClearActions();

            FsmState choice = fsm.GetState("Choice");
            choice.Actions = choice.Actions.Where(a =>
            {
                if (a is PlayerDataBoolTest pdbt)
                {
                    switch (pdbt.boolName.Value)
                    {
                        case nameof(PlayerData.legEaterLeft):
                        case nameof(PlayerData.divineFinalConvo):
                        case nameof(PlayerData.equippedCharm_10):
                            return false;
                    }
                }
                if (a is PlayerDataBoolAllTrue pdbat) return false;
                if (a is PlayerDataBoolTrueAndFalse pdbtaf && pdbtaf.isTrue.Name == "HAS CHARM") return false;
                return true;
            }).ToArray();

            FsmState greet = fsm.GetState("Greet");
            greet.Actions[0] = new Lambda(() =>
            {
                string text = Language.Language.Get("DIVINE_CONVO_1", "CP2");
                List<int> charmIDs = new[] { DivineShopSlot.Heart, DivineShopSlot.Greed, DivineShopSlot.Strength }
                    .Where(s => !PlayerData.instance.GetBool("pooedFragile" + s.ToString()))
                    .Select(s => fsm.FsmVariables.FindFsmInt("Required Charm " + s.ToString()))
                    .Where(fi => fi is not null)
                    .Select(fi => fi.Value).ToList();
                if (charmIDs.Count != 0)
                {
                    text += $"<page>I want it... The smell of {CharmNameUtil.GetCharmName(charmIDs[0])}";
                    for (int i = 1; i < charmIDs.Count; i++) text += $", and {CharmNameUtil.GetCharmName(charmIDs[i])}";
                    text += "... I want it!";
                }
                DialogueCenter.StartConversation(text);
            });

            FsmState pooCharm = fsm.GetState("Poo Charm");
            pooCharm.RemoveActionsOfType<SetBoolValue>();
            pooCharm.AddFirstAction(new Lambda(() => ActivateOnEnd(fsm)));
            pooCharm.AddLastAction(new Wait
            {
                finishEvent = pooCharm.Transitions[0].FsmEvent,
                time = 4f,
                realTime = false,
            });

            FsmState request = GetNewState("Mod Request");
            FsmState boxDown = GetNewState("Mod Dial Box Down");
            FsmState boxUp = GetNewState("Mod Box Up YN");
            FsmState sendText = GetNewState("Mod Send Text");
            FsmState yes = GetNewState("Mod Yes");
            FsmState grab = GetNewState("Mod Grab");
            FsmState boxUp2 = GetNewState("Mod Box Up 2");
            FsmState grabbed = GetNewState("Mod Grabbed");
            FsmState boxDown2 = GetNewState("Mod Box Down 2");
            FsmState eat = GetNewState("Mod Eat");
            FsmState eatEnd = GetNewState("Mod Eat End");
            FsmState eatReturn = GetNewState("Mod Eat Return");
            FsmState swallow = GetNewState("Mod Swallow");

            request.Actions = new FsmStateAction[]
            {
                fsm.GetState("Request Charm").GetFirstActionOfType<AudioPlayerOneShotSingle>(),
            };
            request.AddTransition(FsmEvent.GetFsmEvent("CONVO_FINISH"), boxDown);

            boxDown.Actions = fsm.GetState("Dial Box Down").Actions.ToArray();
            boxDown.AddTransition(FsmEvent.Finished, boxUp);

            boxUp.Actions = fsm.GetState("Box Up YN").Actions.ToArray();
            boxUp.AddTransition(FsmEvent.Finished, sendText);

            sendText.Actions = Array.Empty<FsmStateAction>();
            sendText.AddTransition(FsmEvent.GetFsmEvent("NO"), fsm.GetState("Decline Pause"));
            sendText.AddTransition(FsmEvent.GetFsmEvent("YES"), yes);

            yes.Actions = fsm.GetState("Yes").Actions.ToArray();
            yes.AddTransition(FsmEvent.Finished, grab);

            grab.Actions = fsm.GetState("Take Choice").Actions.Where(a => a is AudioPlayerOneShotSingle || a is Tk2dPlayAnimation)
                .Concat(fsm.GetState("Grab Pause").Actions.Where(a => a is Wait)).ToArray();
            grab.AddTransition(FsmEvent.Finished, boxUp2);

            boxUp2.Actions = fsm.GetState("Box Up 3").Actions.ToArray();
            boxUp2.AddTransition(FsmEvent.Finished, grabbed);

            grabbed.Actions = new FsmStateAction[]
            {
                new Lambda(() => DialogueCenter.StartConversation(Language.Language.Get("DIVINE_GIVE", "CP2"))),
                fsm.GetState("Grabbed").GetFirstActionOfType<AudioPlayerOneShotSingle>(),
            };
            grabbed.AddTransition(FsmEvent.GetFsmEvent("CONVO_FINISH"), boxDown2);

            boxDown2.Actions = fsm.GetState("Dial Box Down 3").Actions.ToArray();
            boxDown2.AddTransition(FsmEvent.Finished, eat);

            eat.Actions = fsm.GetState("Eat").Actions.ToArray();
            eat.AddTransition(FsmEvent.GetFsmEvent("WAIT"), eatEnd);

            eatEnd.Actions = fsm.GetState("Eat End").Actions.ToArray();
            eatEnd.AddTransition(FsmEvent.GetFsmEvent("WAIT"), eatReturn);

            eatReturn.Actions = fsm.GetState("Eat Return").Actions.ToArray();
            eatReturn.AddTransition(FsmEvent.Finished, swallow);

            swallow.Actions = fsm.GetState("Swallow").Actions.ToArray();
            swallow.AddTransition(FsmEvent.Finished, fsm.GetState("Poo 1"));

            FsmState GetNewState(string name)
            {
                FsmState state = new(fsm.Fsm)
                {
                    Name = name,
                    Actions = Array.Empty<FsmStateAction>(),
                };
                fsm.AddState(state);
                return state;
            }
        }

        private void EditDivineFsm(PlayMakerFSM fsm)
        {
            if (fsm.GetState("Mod Request") == null) CreateModdedPath(fsm);
            EditSlotSpecificPath(fsm);
        }
        /*
        // before CreateModdedPath:
        choice.Actions = new FsmStateAction[]
        {
            // choice.Actions[0], // AudioStop
            // choice.Actions[1], // PDBT, divineFinalConvo, FINAL REPEAT
            // choice.Actions[2], // PDBT, legEaterLeft, FINAL
            // choice.Actions[3], // PDBAT, ALL EATEN
            // choice.Actions[4], // PDBT, metDivine, MEET
            // choice.Actions[5], // PDBT, gaveFragileHeart, SWALLOWED HEART
            // choice.Actions[6], // PDBT, gaveFragileGreed, SWALLOWED Greed
            // choice.Actions[7], // PDBT, gaveFragileStrength, SWALLOWED Strength
            // choice.Actions[8], // PDBTAF, EQUIPPED HEART,
            // choice.Actions[9], // PDBTAF, EQUIPPED GREED,
            // choice.Actions[10], // PDBTAF, EQUIPPED STRENGTH,
            // choice.Actions[11], // PDBT, DUNG
            // choice.Actions[12], // PDBTAF, HAS CHARM
            // choice.Actions[13], // PDBTAF, HAS CHARM
            // choice.Actions[14], // PDBTAF, HAS CHARM
        };

        // after CreateModdedPath:
        choice.Actions = new FsmStateAction[]
        {
            // choice.Actions[0], // AudioStop
            // choice.Actions[4], // PDBT, metDivine, MEET
            // choice.Actions[5], // PDBT, gaveFragileHeart, SWALLOWED HEART
            // choice.Actions[6], // PDBT, gaveFragileGreed, SWALLOWED Greed
            // choice.Actions[7], // PDBT, gaveFragileStrength, SWALLOWED Strength
            // choice.Actions[8], // PDBTAF, EQUIPPED HEART,
            // choice.Actions[9], // PDBTAF, EQUIPPED GREED,
            // choice.Actions[10], // PDBTAF, EQUIPPED STRENGTH,
        };

        // The meet PDBT is kept. The remaining PDBTs are deleted and PDBTAFs are replaced by DelegateBoolTests by the corresponding locations.
        */
    }
}
