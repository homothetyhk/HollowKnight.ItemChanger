using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Components;
using ItemChanger.Extensions;
using UnityEngine;
using System.Collections;
using TMPro;
using ItemChanger.Internal;

namespace ItemChanger.Util
{
    public static class ShinyUtility
    {
        public static readonly Color WasEverObtainedColor = new(1f, 213f / 255f, 0.5f);

        /// <summary>
        /// Makes a Shiny Item with a name tied to location and item index. Apply FSM edits in OnEnable instead.
        /// </summary>
        public static GameObject MakeNewShiny(AbstractPlacement placement, AbstractItem item, FlingType flingType)
        {
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = GetShinyName(placement, item);
            var info = shiny.AddComponent<ContainerInfo>();
            info.containerType = Container.Shiny;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = item.Yield(),
                flingType = flingType,
            };
            if (item.WasEverObtained()) shiny.GetComponent<SpriteRenderer>().color = WasEverObtainedColor;

            return shiny;
        }

        public static GameObject MakeNewMultiItemShiny(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null)
        {
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = GetShinyPrefix(placement);
            var info = shiny.AddComponent<ContainerInfo>();
            info.containerType = Container.Shiny;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };

            if (cost != null)
            {
                info.costInfo = new CostInfo
                {
                    cost = cost,
                    previewItems = items,
                };
            }

            if (items.All(i => i.WasEverObtained())) shiny.GetComponent<SpriteRenderer>().color = WasEverObtainedColor;

            return shiny;
        }

        public static bool TryGetItemFromShinyName(string shinyObjectName, AbstractPlacement placement, out AbstractItem item)
        {
            item = null;
            if (!shinyObjectName.StartsWith(GetShinyPrefix(placement))
                || !int.TryParse(shinyObjectName.Split('-').Last(), out int index)
                || index < 0
                || index >= placement.Items.Count()) return false;

            item = placement.Items.ElementAt(index);
            return true;
        }

        public static string GetShinyName(AbstractPlacement placement, AbstractItem item)
        {
            return $"{GetShinyPrefix(placement)}-{item.name}-{placement.Items.TakeWhile(i => i != item).Count()}";
        }

        public static string GetShinyPrefix(AbstractPlacement placement)
        {
            return $"Shiny Item-{placement.Name}";
        }

        public static void PutShinyInContainer(GameObject container, GameObject shiny)
        {
            shiny.SetActive(false);
            shiny.transform.SetParent(container.transform);
            shiny.transform.position = new(container.transform.position.x, container.transform.position.y, 0);
        }

        public static void FlingShinyRandomly(PlayMakerFSM shinyFsm)
        {
            FsmState shinyFling = shinyFsm.GetState("Fling?");
            if (shinyFling.Actions.Length < 10 || shinyFling.Transitions.Length < 6) return; // Fling? has already been edited.
            shinyFling.Actions = new FsmStateAction[]
            {
                // shinyFling.Actions[0], // BoolTest -- Fling on start
                shinyFling.Actions[1], // PlayParticleEmitter
                shinyFling.Actions[2], // SetFsmBool -- Inspect Region must be disabled!!!
                shinyFling.Actions[3], // SetGravity2dScale
                // shinyFling.Actions[4-8], // Cases for different fling types
                shinyFling.Actions[9] // SendRandomEvent -- "Fling L" "Fling R"
            };
        }

        public static void FlingShinyDown(PlayMakerFSM shinyFsm)
        {
            FsmState fling = shinyFsm.GetState("Fling?");
            fling.ClearTransitions();
            fling.AddTransition("FINISHED", "Fling R");
            FlingObject flingObj = shinyFsm.GetState("Fling R").GetActionsOfType<FlingObject>()[0];
            flingObj.angleMin = flingObj.angleMax = 270;
            flingObj.speedMin = flingObj.speedMax = 0.1f;
        }

        public static void AddChangeSceneToShiny(PlayMakerFSM shinyFsm, string toScene, string toGate)
        {
            if (toGate == ChangeSceneInfo.door_dreamReturn)
            {
                shinyFsm.FsmVariables.FindFsmBool("Exit Dream").Value = true;
                shinyFsm.GetState("Fade Pause").AddFirstAction(new Lambda(() =>
                {
                    PlayerData.instance.SetString(nameof(PlayerData.dreamReturnScene), toScene);
                }));
            }
            else
            {
                FsmState finish = shinyFsm.GetState("Finish");
                finish.AddLastAction(new ChangeSceneAction(toScene, toGate));
            }
        }

        public static void ModifyShiny(PlayMakerFSM shinyFsm, FlingType flingType, AbstractPlacement placement, AbstractItem item)
        {
            FsmState pdBool = shinyFsm.GetState("PD Bool?");
            FsmState charm = shinyFsm.GetState("Charm?");
            FsmState trinkFlash = shinyFsm.GetState("Trink Flash");

            GiveInfo info = new GiveInfo
            {
                Container = Container.Shiny,
                FlingType = flingType,
                Transform = shinyFsm.transform,
                MessageType = MessageType.Any,
                Callback = _ => shinyFsm.SendEvent("GAVE ITEM"),
            };
            FsmStateAction checkAction = new Lambda(() => shinyFsm.SendEvent(item.IsObtained() ? "COLLECTED" : null));
            FsmStateAction giveAction = new Lambda(() => item.Give(placement, info));

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<StringCompare>();

            // Change pd bool test to our new bool
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.AddLastAction(checkAction);

            // Charm must be preserved as the entry point for AddYNDialogueToShiny
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Trink Flash");

            trinkFlash.ClearTransitions();
            trinkFlash.Actions = new FsmStateAction[]
            {
                trinkFlash.Actions[0], // Audio
                trinkFlash.Actions[1], // Audio
                trinkFlash.Actions[2], // visual effect
                trinkFlash.Actions[3], // hide shiny
                trinkFlash.Actions[4], // pickup animation
                // [5] -- spawn message
                // [6] -- store message text
                // [7] -- store message icon
                giveAction, // give item and await callback
            };
            trinkFlash.AddTransition("GAVE ITEM", "Hero Up");
        }

        public static void ModifyMultiShiny(PlayMakerFSM shinyFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            FsmState init = shinyFsm.GetState("Init");
            FsmState pdBool = shinyFsm.GetState("PD Bool?");
            FsmState charm = shinyFsm.GetState("Charm?");
            FsmState trinkFlash = shinyFsm.GetState("Trink Flash");

            GiveInfo info = new GiveInfo
            {
                Container = Container.Shiny,
                FlingType = flingType,
                Transform = shinyFsm.transform,
                MessageType = MessageType.Any,
            };
            FsmStateAction checkAction = new Lambda(() => shinyFsm.SendEvent(items.All(i => i.IsObtained()) ? "COLLECTED" : null));
            FsmStateAction giveAction = new Lambda(() => ItemUtility.GiveSequentially(items, placement, info, callback: () => shinyFsm.SendEvent("GAVE ITEM")));

            // Remove actions that stop shiny from spawning
            init.RemoveActionsOfType<BoolTest>();
            pdBool.ClearActions();

            // Change pd bool test to our new bool
            pdBool.AddLastAction(checkAction);

            // Charm must be preserved as the entry point for AddYNDialogueToShiny
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Trink Flash");

            trinkFlash.ClearTransitions();
            trinkFlash.Actions = new FsmStateAction[]
            {
                trinkFlash.Actions[0], // Audio
                trinkFlash.Actions[1], // Audio
                trinkFlash.Actions[2], // visual effect
                trinkFlash.Actions[3], // hide shiny
                trinkFlash.Actions[4], // pickup animation
                // [5] -- spawn message
                // [6] -- store message text
                // [7] -- store message icon
                giveAction, // give item
            };
            trinkFlash.AddTransition("GAVE ITEM", "Hero Up");
        }

        /// <summary>
        /// Call after ModifyShiny to add cost.
        /// </summary>
        public static void AddYNDialogueToShiny(PlayMakerFSM shinyFsm, Cost cost, IEnumerable<AbstractItem> items)
        {
            FsmState charm = shinyFsm.GetState("Charm?");
            FsmState yesState = shinyFsm.GetState(charm.Transitions[0].ToState);
            FsmState noState = new FsmState(shinyFsm.GetState("Idle"))
            {
                Name = "YN No"
            };
            FsmState giveControl = new FsmState(shinyFsm.GetState("Idle"))
            {
                Name = "Give Control"
            };

            FsmStateAction closeYNDialogue = new Lambda(() => YNUtil.CloseYNDialogue());

            noState.ClearTransitions();
            noState.RemoveActionsOfType<FsmStateAction>();
            noState.AddTransition(FsmEvent.Finished, giveControl);
            noState.AddTransition("HERO DAMAGED", giveControl);

            Tk2dPlayAnimationWithEvents heroUp = new Tk2dPlayAnimationWithEvents
            {
                gameObject = new FsmOwnerDefault
                {
                    OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                    GameObject = HeroController.instance.gameObject
                },
                clipName = "Collect Normal 3",
                animationTriggerEvent = null,
                animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED")
            };

            noState.AddLastAction(closeYNDialogue);
            noState.AddLastAction(heroUp);

            giveControl.ClearTransitions();
            giveControl.RemoveActionsOfType<FsmStateAction>();

            giveControl.AddTransition("FINISHED", "Idle");

            giveControl.AddLastAction(new Lambda(() => PlayMakerFSM.BroadcastEvent("END INSPECT")));

            shinyFsm.AddState(noState);
            shinyFsm.AddState(giveControl);

            charm.ClearTransitions();

            charm.AddTransition("HERO DAMAGED", noState.Name);
            charm.AddTransition("NO", noState.Name);
            charm.AddTransition("YES", yesState.Name);

            yesState.AddFirstAction(new Lambda(() => cost.Pay()));
            yesState.AddFirstAction(closeYNDialogue);

            charm.AddFirstAction(new Lambda(() => OpenYNDialogue(shinyFsm.gameObject, items, cost)));
        }

        private static void OpenYNDialogue(GameObject shiny, IEnumerable<AbstractItem> items, Cost cost)
        {
            // If the text pushes the Y/N buttons off of the page, it results in an input lock
            // 120 characters is a little generous--all MMMMMs will still overflow
            string itemText = string.Join(", ", items.Select(i => i.UIDef.GetPostviewName()).ToArray());
            if (itemText.Length > 120)
            {
                itemText = itemText.Substring(0, 117) + "...";
            }

            string costText = cost.GetCostText();
            if (costText.Length > 40)
            {
                costText = costText.Substring(0, 37) + "...";
            }

            YNUtil.OpenYNDialogue(shiny, $"{itemText}\n{costText}", cost.CanPay());
        }

        
    }
}
