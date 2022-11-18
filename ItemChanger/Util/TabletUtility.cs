using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Extensions;

namespace ItemChanger.Util
{
    public static class TabletUtility
    {
        public static GameObject InstantiateTablet(AbstractPlacement placement) => InstantiateTablet(GetTabletName(placement));

        public static GameObject InstantiateTablet(string tabletName)
        {
            GameObject tablet = ObjectCache.LoreTablet;

            GameObject lit_tablet = tablet.transform.Find("lit_tablet").gameObject; // doesn't appear after instantiation, for some reason
            GameObject lit = new GameObject();
            lit.transform.SetParent(tablet.transform);
            lit.transform.localPosition = new Vector3(-0.1f, 0.1f, -3f);
            lit.transform.localScale = Vector3.one;
            lit.AddComponent<SpriteRenderer>().sprite = lit_tablet.GetComponent<SpriteRenderer>().sprite;
            lit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);

            tablet.name = tabletName;

            return tablet;
        }

        public static GameObject MakeNewTablet(AbstractPlacement placement)
        {
            GameObject tablet = InstantiateTablet(placement);
            tablet.SetActive(true);

            return tablet;
        }

        public static GameObject MakeNewTablet(AbstractPlacement placement, Func<string> textGenerator)
        {
            return MakeNewTablet(GetTabletName(placement), textGenerator);
        }    
        
        /// <summary>
        /// Creates a lore tablet GameObject with the specified name and lore text.
        /// </summary>
        /// <param name="tabletName">The name of the tablet.</param>
        /// <param name="textGenerator">This method is invoked when the player reads the lore tablet to set the displayed text.</param>
        /// <returns>The tablet GameObject.</returns>
        public static GameObject MakeNewTablet(string tabletName, Func<string> textGenerator)
        {
            GameObject tablet = InstantiateTablet(tabletName);
            tablet.SetActive(true);

            PlayMakerFSM inspectFsm = tablet.LocateMyFSM("Inspection");

            FsmState promptUp = inspectFsm.GetState("Prompt Up");
            promptUp.SetActions(
                    promptUp.Actions[0], // AudioStop
                    promptUp.Actions[1], // TurnToBG
                    promptUp.Actions[2], // lore tablet audio clip
                    promptUp.Actions[3], // vibration
                    //promptUp.Actions[4], // change text align
                    //promptUp.Actions[5], // move text
                    //promptUp.Actions[6], // HUD Canvas OUT
                    //promptUp.Actions[7], // LORE PROMPT UP
                    new AsyncLambda(callback => DialogueCenter.SendLoreMessage(
                        textGenerator?.Invoke() ?? string.Empty,
                        callback,
                        TextType.MajorLore), "CONVO_FINISH")
            );
            FsmState setBool = inspectFsm.GetState("Set Bool");
            FsmState turnBack = inspectFsm.GetState("Turn Back");
            promptUp.ClearTransitions();
            promptUp.AddTransition("CONVO_FINISH", turnBack);
            foreach (var t in setBool.Transitions) t.SetToState(turnBack);

            return tablet;
        }

        public static GameObject MakeNewTablet(ContainerInfo info)
        {
            GameObject tablet = InstantiateTablet(info.giveInfo.placement);
            tablet.AddComponent<ContainerInfoComponent>().info = info;
            return tablet;
        }

        internal static GameObject MakeNewTablet(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return MakeNewTablet(new ContainerInfo(Container.Tablet, placement, items, flingType));
        }


        public static void ModifyTablet(PlayMakerFSM inspectFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            AddItemParticles(inspectFsm.gameObject, placement, items);

            if (inspectFsm.FsmName == "Inspection")
            {
                FsmState init = inspectFsm.GetState("Init");
                FsmState regainControl = inspectFsm.GetState("Regain Control");
                FsmState outOfRange = inspectFsm.GetState("Out Of Range");

                void DisableInspect()
                {
                    if (items.All(i => i.IsObtained()))
                    {
                        outOfRange.ClearTransitions();
                    }
                }

                init.GetFirstActionOfType<SetFsmString>().setValue = "Accept";

                init.AddLastAction(new Lambda(DisableInspect));
                regainControl.AddLastAction(new Lambda(DisableInspect));

                FsmState promptUp = inspectFsm.GetState("Prompt Up");
                promptUp.SetActions(
                    promptUp.Actions[0], // AudioStop
                    promptUp.Actions[1], // TurnToBG
                    promptUp.Actions[2], // lore tablet audio clip
                    promptUp.Actions[3], // vibration
                    //promptUp.Actions[4], // change text align
                    //promptUp.Actions[5], // move text
                    //promptUp.Actions[6], // HUD Canvas OUT
                    //promptUp.Actions[7], // LORE PROMPT UP
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(items, placement, new GiveInfo
                    {
                        FlingType = flingType,
                        Container = Container.Tablet,
                        MessageType = MessageType.Any,
                        Transform = inspectFsm.transform,
                    }, callback), "CONVO_FINISH")
                );
                FsmState setBool = inspectFsm.GetState("Set Bool");
                FsmState turnBack = inspectFsm.GetState("Turn Back");
                promptUp.ClearTransitions();
                promptUp.AddTransition("CONVO_FINISH", turnBack);

                // the abyss tablet doesn't have the hero damaged behavior, so we add it back in for consistency
                if (setBool == null)
                {
                    if (inspectFsm.FsmVariables.FindFsmBool("Hero Damaged") is not FsmBool heroDamaged)
                    {
                        heroDamaged = inspectFsm.AddFsmBool("Hero Damaged", false);
                    }
                    turnBack.AddFirstAction(new BoolTest
                    {
                        boolVariable = heroDamaged,
                        isFalse = null,
                        isTrue = FsmEvent.Finished,
                    });
                    setBool = new FsmState(inspectFsm.Fsm)
                    {
                        Name = "Set Bool",
                        Transitions = new FsmTransition[] { new FsmTransition { FsmEvent = FsmEvent.Finished, ToFsmState = inspectFsm.GetState("Down"), ToState = "Down", } },
                    };
                    setBool.SetActions(
                        new SetBoolValue { boolVariable = heroDamaged, boolValue = true }
                    );
                    inspectFsm.AddState(setBool);
                    inspectFsm.Fsm.GlobalTransitions = inspectFsm.Fsm.GlobalTransitions.Prepend(new FsmTransition
                    {
                        FsmEvent = FsmEvent.GetFsmEvent("HERO DAMAGED"),
                        ToFsmState = setBool,
                        ToState = setBool.Name,
                    }).ToArray();
                    inspectFsm.GetState("Take Control").AddFirstAction(new SetBoolValue { boolVariable = heroDamaged, boolValue = false });
                }
                foreach (var t in setBool.Transitions) t.SetToState(turnBack);
            }
            else if (inspectFsm.FsmName == "inspect_region")
            {
                FsmState heroLookUp = inspectFsm.GetState("Hero Look Up?");
                FsmState cancel = inspectFsm.GetState("Cancel");
                FsmState convoEnd = inspectFsm.GetState("Convo End");
                FsmState canTalkBool = inspectFsm.GetState("Can Talk Bool?");

                inspectFsm.FsmVariables.FindFsmString("Prompt Name").Value = "Accept";

                foreach (var t in heroLookUp.Transitions) t.SetToState(cancel);
                cancel.SetActions(
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(items, placement, new GiveInfo
                    {
                        FlingType = flingType,
                        Container = Container.Tablet,
                        MessageType = MessageType.Any,
                        Transform = inspectFsm.transform,
                    }, callback))
                );
                convoEnd.RemoveActionsOfType<SetTextMeshProAlignment>();
                canTalkBool.AddFirstAction(new DelegateBoolTest(() => items.All(i => i.IsObtained()), "FALSE", null));
            }
            else if (inspectFsm.FsmName == "Conversation Control" && inspectFsm.GetState("Journal") is FsmState journal)
            {
                journal.SetActions(
                    new DelegateBoolTest(placement.AllObtained, journal.GetFirstActionOfType<PlayerDataBoolTest>()),
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(items, placement, new GiveInfo
                    {
                        FlingType = flingType,
                        Container = Container.Tablet,
                        MessageType = MessageType.Any,
                        Transform = inspectFsm.transform,
                    }, callback))
                );
            }
        }

        public static void AddItemParticles(GameObject parent, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            Tags.TabletParticleControlTag tpct = placement.GetPlacementAndLocationTags().OfType<Tags.TabletParticleControlTag>().FirstOrDefault();
            if (tpct == null || !tpct.forceDisableParticles)
            {
                var ip = parent.AddComponent<ItemParticles>();
                ip.items = items;
                Vector3 offset = Vector3.zero;
                if (tpct != null) offset += new Vector3(tpct.offsetX, tpct.offsetY, tpct.offsetZ);
                if (placement is Placements.ExistingContainerPlacement ecp && ecp.Location is Locations.ExistingFsmContainerLocation efcl)
                {
                    offset.y -= efcl.elevation;
                }
                ip.offset = offset;
            }
        }


        public static string GetTabletName(AbstractPlacement placement)
        {
            return $"Lore Tablet-{placement.Name}";
        }
    }
}
