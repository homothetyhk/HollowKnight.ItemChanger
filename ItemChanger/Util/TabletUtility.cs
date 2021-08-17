using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Extensions;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class TabletUtility
    {
        public static GameObject MakeNewTablet(AbstractPlacement placement)
        {
            GameObject tablet = ObjectCache.LoreTablet;

            GameObject lit_tablet = tablet.transform.Find("lit_tablet").gameObject; // doesn't appear after instantiation, for some reason
            GameObject lit = new GameObject();
            lit.transform.SetParent(tablet.transform);
            lit.transform.localPosition = new Vector3(-0.1f, 0.1f, -1.8f);
            lit.transform.localScale = Vector3.one;
            lit.AddComponent<SpriteRenderer>().sprite = lit_tablet.GetComponent<SpriteRenderer>().sprite;
            lit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);

            tablet.name = GetTabletName(placement);
            tablet.SetActive(true);



            return tablet;
        }

        public static GameObject MakeNewTablet(AbstractPlacement placement, Func<string> textGenerator)
        {
            GameObject tablet = ObjectCache.LoreTablet;

            GameObject lit_tablet = tablet.transform.Find("lit_tablet").gameObject; // doesn't appear after instantiation, for some reason
            GameObject lit = new GameObject();
            lit.transform.SetParent(tablet.transform);
            lit.transform.localPosition = new Vector3(-0.1f, 0.1f, -1.8f);
            lit.transform.localScale = Vector3.one;
            lit.AddComponent<SpriteRenderer>().sprite = lit_tablet.GetComponent<SpriteRenderer>().sprite;
            lit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);

            tablet.name = GetTabletName(placement);
            tablet.SetActive(true);

            PlayMakerFSM inspectFsm = tablet.LocateFSM("Inspection");

            FsmState promptUp = inspectFsm.GetState("Prompt Up");
            promptUp.Actions = new FsmStateAction[]
            {
                    promptUp.Actions[0], // AudioStop
                    promptUp.Actions[1], // TurnToBG
                    promptUp.Actions[2], // lore tablet audio clip
                    promptUp.Actions[3], // vibration
                    //promptUp.Actions[4], // change text align
                    //promptUp.Actions[5], // move text
                    promptUp.Actions[6], // HUD Canvas OUT
                    //promptUp.Actions[7], // LORE PROMPT UP
                    new AsyncLambda(callback => DialogueCenter.SendLoreMessage(
                        textGenerator?.Invoke() ?? string.Empty,
                        callback,
                        TextType.MajorLore)),
            };
            FsmState setBool = inspectFsm.GetState("Set Bool");
            FsmState turnBack = inspectFsm.GetState("Turn Back");
            foreach (var t in promptUp.Transitions) t.SetToState(turnBack);
            foreach (var t in setBool.Transitions) t.SetToState(turnBack);


            return tablet;
        }

        internal static GameObject MakeNewTablet(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GameObject tablet = ObjectCache.LoreTablet;

            GameObject lit_tablet = tablet.transform.Find("lit_tablet").gameObject; // doesn't appear after instantiation, for some reason
            GameObject lit = new GameObject();
            lit.transform.SetParent(tablet.transform);
            lit.transform.localPosition = new Vector3(-0.1f, 0.1f, -1.8f);
            lit.transform.localScale = Vector3.one;
            lit.AddComponent<SpriteRenderer>().sprite = lit_tablet.GetComponent<SpriteRenderer>().sprite;
            lit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);

            tablet.name = GetTabletName(placement);
            var info = tablet.AddComponent<ContainerInfo>();
            info.containerType = Container.Tablet;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };

            return tablet;
        }


        public static void ModifyTablet(PlayMakerFSM inspectFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
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

                init.AddLastAction(new Lambda(DisableInspect));
                regainControl.AddLastAction(new Lambda(DisableInspect));

                FsmState promptUp = inspectFsm.GetState("Prompt Up");
                promptUp.Actions = new FsmStateAction[]
                {
                    promptUp.Actions[0], // AudioStop
                    promptUp.Actions[1], // TurnToBG
                    promptUp.Actions[2], // lore tablet audio clip
                    promptUp.Actions[3], // vibration
                    //promptUp.Actions[4], // change text align
                    //promptUp.Actions[5], // move text
                    promptUp.Actions[6], // HUD Canvas OUT
                    //promptUp.Actions[7], // LORE PROMPT UP
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(items, placement, new GiveInfo
                    {
                        FlingType = flingType,
                        Container = Container.Tablet,
                        MessageType = MessageType.Any,
                        Transform = inspectFsm.transform,
                    }, callback)),
                };
                FsmState setBool = inspectFsm.GetState("Set Bool");
                FsmState turnBack = inspectFsm.GetState("Turn Back");
                foreach (var t in promptUp.Transitions) t.SetToState(turnBack);
                foreach (var t in setBool.Transitions) t.SetToState(turnBack);
            }
            else if (inspectFsm.FsmName == "inspect_region")
            {
                FsmState heroLookUp = inspectFsm.GetState("Hero Look Up?");
                FsmState cancel = inspectFsm.GetState("Cancel");
                FsmState convoEnd = inspectFsm.GetState("Convo End");
                FsmState canTalkBool = inspectFsm.GetState("Can Talk Bool?");

                foreach (var t in heroLookUp.Transitions) t.SetToState(cancel);
                cancel.Actions = new FsmStateAction[]
                {
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(items, placement, new GiveInfo
                        {
                            FlingType = flingType,
                            Container = Container.Tablet,
                            MessageType = MessageType.Any,
                            Transform = inspectFsm.transform,
                        }, callback)),
                };
                convoEnd.RemoveActionsOfType<SetTextMeshProAlignment>();
                canTalkBool.AddFirstAction(new BoolTestMod(() => items.All(i => i.IsObtained()), "FALSE", null));
            }
        }

        public static string GetTabletName(AbstractPlacement placement)
        {
            return $"Lore Tablet-{placement.Name}";
        }


    }
}
