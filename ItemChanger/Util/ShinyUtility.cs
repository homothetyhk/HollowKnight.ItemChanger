using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Components;
using SereCore;
using UnityEngine;
using System.Collections;
using TMPro;

namespace ItemChanger.Util
{
    public static class ShinyUtility
    {
        /// <summary>
        /// Makes a Shiny Item with a name tied to location and item index. Apply FSM edits in OnEnable instead.
        /// </summary>
        public static GameObject MakeNewShiny(AbstractPlacement placement, AbstractItem item)
        {
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = GetShinyName(placement, item);
            var info = shiny.AddComponent<ContainerInfo>();
            info.placement = placement;
            info.items = item.Yield();
            info.flingType = placement.Location.flingType;

            return shiny;
        }

        public static GameObject MakeNewMultiItemShiny(AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = GetShinyPrefix(placement);
            var info = shiny.AddComponent<ContainerInfo>();
            info.placement = placement;
            info.items = items;
            info.flingType = placement.Location.flingType;

            if (placement is Placements.ISingleCostPlacement costPlacement)
            {
                shiny.AddComponent<CostInfo>().cost = costPlacement.Cost;
            }

            return shiny;
        }

        public static void ModifyFsm(PlayMakerFSM shinyFsm)
        {
            ContainerInfo containerInfo = shinyFsm.gameObject.GetComponent<ContainerInfo>();
            CostInfo costInfo = shinyFsm.gameObject.GetComponent<CostInfo>();
            ChangeSceneInfo changeSceneInfo = shinyFsm.gameObject.GetComponent<ChangeSceneInfo>();

            if (containerInfo && !containerInfo.applied)
            {
                ModifyMultiShiny(shinyFsm, containerInfo.flingType, containerInfo.placement, containerInfo.items);
                containerInfo.applied = true;
            }

            if (costInfo && !costInfo.applied)
            {
                AddYNDialogueToShiny(shinyFsm, costInfo.cost, containerInfo?.items);
                costInfo.applied = true;
            }

            if (changeSceneInfo && !changeSceneInfo.applied)
            {
                AddChangeSceneToShiny(shinyFsm, changeSceneInfo.toScene, changeSceneInfo.toGate);
                changeSceneInfo.applied = true;
            }
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
            shiny.transform.position = container.transform.position;
        }

        public static void FlingShinyRandomly(PlayMakerFSM shinyFsm)
        {
            FsmState shinyFling = shinyFsm.GetState("Fling?");
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
            if (toGate == "door_DreamReturn")
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
                finish.AddAction(new RandomizerChangeScene(toScene, toGate));
            }
        }

        public static void ModifyShiny(PlayMakerFSM shinyFsm, FlingType flingType, AbstractPlacement placement, AbstractItem item)
        {
            ItemChanger.instance.Log("Single Shiny!");

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
            pdBool.AddAction(checkAction);

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
            pdBool.RemoveActionsOfType<StringCompare>();

            // Change pd bool test to our new bool
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.AddAction(checkAction);

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

            FsmStateAction closeYNDialogue = new Lambda(() => CloseYNDialogue());

            noState.ClearTransitions();
            noState.RemoveActionsOfType<FsmStateAction>();
            noState.AddTransition("FINISHED", "Give Control");
            noState.AddTransition("HERO DAMAGED", "Give Control");

            Tk2dPlayAnimationWithEvents heroUp = new Tk2dPlayAnimationWithEvents
            {
                gameObject = new FsmOwnerDefault
                {
                    OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                    GameObject = SereCore.Ref.Hero.gameObject
                },
                clipName = "Collect Normal 3",
                animationTriggerEvent = null,
                animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED")
            };

            noState.AddAction(closeYNDialogue);
            noState.AddAction(heroUp);

            giveControl.ClearTransitions();
            giveControl.RemoveActionsOfType<FsmStateAction>();

            giveControl.AddTransition("FINISHED", "Idle");

            giveControl.AddAction(new Lambda(() => PlayMakerFSM.BroadcastEvent("END INSPECT")));

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
            FSMUtility.LocateFSM(GameObject.Find("DialogueManager"), "Box Open YN").SendEvent("BOX UP YN");
            FSMUtility.LocateFSM(GameObject.Find("Text YN"), "Dialogue Page Control").FsmVariables
                .GetFsmGameObject("Requester").Value = shiny;

            // If the text pushes the Y/N buttons off of the page, it results in an input lock
            // These lengths are a little generous--all MMMMMs will still overflow
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

            LanguageStringManager.SetString("UI", "RANDOMIZER_YN_DIALOGUE", $"{itemText}<br>{costText}");

            if (!cost.CanPay())
            {
                FSMUtility.LocateFSM(GameObject.Find("Text YN"), "Dialogue Page Control")
                            .StartCoroutine(KillGeoText());
            }

            FSMUtility.LocateFSM(GameObject.Find("Text YN"), "Dialogue Page Control").FsmVariables
                .GetFsmInt("Toll Cost").Value = 0;
            FSMUtility.LocateFSM(GameObject.Find("Text YN"), "Dialogue Page Control").FsmVariables
                .GetFsmGameObject("Geo Text").Value.SetActive(true);

            GameObject.Find("Text YN").GetComponent<DialogueBox>().StartConversation("RANDOMIZER_YN_DIALOGUE", "UI");
        }

        private static void CloseYNDialogue()
        {
            FSMUtility.LocateFSM(GameObject.Find("DialogueManager"), "Box Open YN").SendEvent("BOX DOWN YN");
        }

        private static IEnumerator KillGeoText()
        {
            PlayMakerFSM ynFsm = FSMUtility.LocateFSM(GameObject.Find("Text YN"), "Dialogue Page Control");
            while (ynFsm.ActiveStateName != "Ready for Input")
            {
                yield return new WaitForEndOfFrame();
            }

            ynFsm.FsmVariables.GetFsmGameObject("Geo Text").Value.SetActive(false);
            ynFsm.FsmVariables.GetFsmInt("Toll Cost").Value = int.MaxValue;
            PlayMakerFSM.BroadcastEvent("NOT ENOUGH");
        }

    }
}
