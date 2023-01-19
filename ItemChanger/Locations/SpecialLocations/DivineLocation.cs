using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Util;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which replaces the shinies inside Divine's room. Each of the 3 shinies requires its own location with this implementation.
    /// </summary>
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
            ObjectLocation.FindGameObject("Divine NPC").LocateMyFSM("Conversation Control").AddFsmGameObject("Replace Object " + shopSlot.ToString(), obj);
        }

        private bool ShouldGiveItem()
        {
            return PlayerData.instance.GetBool("equippedCharm_" + requiredCharmID) && !Placement.CheckVisitedAny(VisitState.Accepted) && !Placement.AllObtained();
        }

        private bool ItemIsPooed()
        {
            return Placement.CheckVisitedAny(VisitState.Accepted) && !Placement.AllObtained();
        }

        private Cost? GetCost()
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
            pooed.ReplaceAction(new DelegateBoolTest(ItemIsPooed, FsmEvent.GetFsmEvent("SPAWN"), FsmEvent.Finished), 0);

            FsmState spawnReady = fsm.GetState("Spawn Ready");
            spawnReady.ReplaceAction(new Lambda(() => ActivateIfModdedPooed(fsm)), 1 + (int)shopSlot);

            FsmState choice = fsm.GetState("Choice");
            string gaveBool = "gaveFragile" + shopSlot.ToString();
            int i = Array.FindIndex(choice.Actions, a => a is PlayerDataBoolTest pdbt && pdbt.boolName.Value == gaveBool);
            choice.RemoveAction(i);
            string equipEvent = "EQUIPPED " + (shopSlot == DivineShopSlot.Greed ? "GEO" : shopSlot.ToString().ToUpper());
            i = Array.FindIndex(choice.Actions, a => a is PlayerDataBoolTrueAndFalse pdbtaf && pdbtaf.isTrue?.Name == equipEvent);
            choice.ReplaceAction(new DelegateBoolTest(ShouldGiveItem, FsmEvent.GetFsmEvent(equipEvent), null), i);

            FsmState request = fsm.GetState("Mod Request");
            request.AddFirstAction(new Lambda(() =>
            {
                if (IsOnThisSlot(fsm))
                {
                    DialogueCenter.StartConversation(string.Format(Language.Language.Get("DIVINE_HAS_CHARM", "Fmt"), CharmNameUtil.GetCharmName(requiredCharmID)));
                }
            }));

            FsmState afterChoice = new(fsm.Fsm)
            {
                Name = "Mod Request " + shopSlot.ToString(),
            };
            afterChoice.SetActions(
                new Lambda(() => fsm.FsmVariables.FindFsmInt("Current Charm").Value = 1 + (int)shopSlot)
            );
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
                    Cost? c = GetCost();
                    if (c is not null && !c.Paid) c.Pay();
                    Placement.AddVisitFlag(VisitState.Accepted);
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
            if (obj.LocateMyFSM("Shiny Control") is PlayMakerFSM shinyFsm) ShinyUtility.FlingShinyLeft(shinyFsm);
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
                return fsm.FsmVariables.FindFsmGameObject("Charm Holder").Value.FindChild("Poo " + slot.ToString())!;
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
            spawnReady.SetActions(
                spawnReady.Actions[0], // SetCollider false
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Heart)), // actions to be replaced by the individual placements
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Greed)),
                new Lambda(() => ActivateShinyIfVanillaPooed(fsm, DivineShopSlot.Strength)),
                new Wait
                {
                    time = 4f,
                    realTime = false,
                    finishEvent = spawnReady.Transitions[0].FsmEvent,
                }
            );
            foreach (string s in new[] { "Spawn Heart", "Spawn Greed", "Spawn Strength" }) fsm.GetState(s).ClearActions();

            FsmState choice = fsm.GetState("Choice");
            choice.SetActions(choice.Actions.Where(a =>
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
            }).ToArray());

            FsmState greet = fsm.GetState("Greet");
            greet.ReplaceAction(new Lambda(() =>
            {
                string text = Language.Language.Get("DIVINE_CONVO_1", "CP2");
                List<int> charmIDs = new[] { DivineShopSlot.Heart, DivineShopSlot.Greed, DivineShopSlot.Strength }
                    .Where(s => !PlayerData.instance.GetBool("pooedFragile" + s.ToString()))
                    .Select(s => fsm.FsmVariables.FindFsmInt("Required Charm " + s.ToString()))
                    .Where(fi => fi is not null)
                    .Select(fi => fi.Value).ToList();
                if (charmIDs.Count != 0)
                {
                    text += string.Format(Language.Language.Get("DIVINE_NO_CHARM", "Fmt"), string.Join(Language.Language.Get("COMMA_AND", "IC"), charmIDs.Select(i => CharmNameUtil.GetCharmName(i))));
                }
                DialogueCenter.StartConversation(text);
            }), 0);

            FsmState pooCharm = fsm.GetState("Poo Charm");
            pooCharm.RemoveActionsOfType<SetBoolValue>();
            pooCharm.AddFirstAction(new Lambda(() => ActivateOnEnd(fsm)));
            pooCharm.AddLastAction(new Wait
            {
                finishEvent = pooCharm.Transitions[0].FsmEvent,
                time = 4f,
                realTime = false,
            });

            FsmState request = fsm.AddState("Mod Request");
            FsmState boxDown = fsm.AddState("Mod Dial Box Down");
            FsmState boxUp = fsm.AddState("Mod Box Up YN");
            FsmState sendText = fsm.AddState("Mod Send Text");
            FsmState yes = fsm.AddState("Mod Yes");
            FsmState grab = fsm.AddState("Mod Grab");
            FsmState boxUp2 = fsm.AddState("Mod Box Up 2");
            FsmState grabbed = fsm.AddState("Mod Grabbed");
            FsmState boxDown2 = fsm.AddState("Mod Box Down 2");
            FsmState eat = fsm.AddState("Mod Eat");
            FsmState eatEnd = fsm.AddState("Mod Eat End");
            FsmState eatReturn = fsm.AddState("Mod Eat Return");
            FsmState swallow = fsm.AddState("Mod Swallow");

            request.SetActions(
                fsm.GetState("Request Charm").GetFirstActionOfType<AudioPlayerOneShotSingle>()
            );
            request.AddTransition(FsmEvent.GetFsmEvent("CONVO_FINISH"), boxDown);

            boxDown.SetActions(fsm.GetState("Dial Box Down").Actions.ToArray());
            boxDown.AddTransition(FsmEvent.Finished, boxUp);

            boxUp.SetActions(fsm.GetState("Box Up YN").Actions.ToArray());
            boxUp.AddTransition(FsmEvent.Finished, sendText);

            sendText.ClearActions();
            sendText.AddTransition(FsmEvent.GetFsmEvent("NO"), fsm.GetState("Decline Pause"));
            sendText.AddTransition(FsmEvent.GetFsmEvent("YES"), yes);

            yes.SetActions(fsm.GetState("Yes").Actions.ToArray());
            yes.AddTransition(FsmEvent.Finished, grab);

            grab.SetActions(fsm.GetState("Take Choice").Actions.Where(a => a is AudioPlayerOneShotSingle || a is Tk2dPlayAnimation)
                .Concat(fsm.GetState("Grab Pause").Actions.Where(a => a is Wait)).ToArray());
            grab.AddTransition(FsmEvent.Finished, boxUp2);

            boxUp2.SetActions(fsm.GetState("Box Up 3").Actions.ToArray());
            boxUp2.AddTransition(FsmEvent.Finished, grabbed);

            grabbed.SetActions(
                new Lambda(() => DialogueCenter.StartConversation(Language.Language.Get("DIVINE_GIVE", "CP2"))),
                fsm.GetState("Grabbed").GetFirstActionOfType<AudioPlayerOneShotSingle>()
            );
            grabbed.AddTransition(FsmEvent.GetFsmEvent("CONVO_FINISH"), boxDown2);

            boxDown2.SetActions(fsm.GetState("Dial Box Down 3").Actions.ToArray());
            boxDown2.AddTransition(FsmEvent.Finished, eat);

            eat.SetActions(fsm.GetState("Eat").Actions.ToArray());
            eat.AddTransition(FsmEvent.GetFsmEvent("WAIT"), eatEnd);

            eatEnd.SetActions(fsm.GetState("Eat End").Actions.ToArray());
            eatEnd.AddTransition(FsmEvent.GetFsmEvent("WAIT"), eatReturn);

            eatReturn.SetActions(fsm.GetState("Eat Return").Actions.ToArray());
            eatReturn.AddTransition(FsmEvent.Finished, swallow);

            swallow.SetActions(fsm.GetState("Swallow").Actions.ToArray());
            swallow.AddTransition(FsmEvent.Finished, fsm.GetState("Poo 1"));
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
