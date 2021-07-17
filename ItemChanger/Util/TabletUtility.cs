using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using SereCore;
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
                    if (placement.AllObtained())
                    {
                        outOfRange.ClearTransitions();
                    }
                }

                init.AddAction(new Lambda(DisableInspect));
                regainControl.AddAction(new Lambda(DisableInspect));

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
                foreach (var t in promptUp.Transitions) t.ToState = "Turn Back";
                foreach (var t in setBool.Transitions) t.ToState = "Turn Back";
            }
            else if (inspectFsm.FsmName == "inspect_region")
            {
                FsmState heroLookUp = inspectFsm.GetState("Hero Look Up?");
                FsmState cancel = inspectFsm.GetState("Cancel");
                FsmState convoEnd = inspectFsm.GetState("Convo End");
                FsmState canTalkBool = inspectFsm.GetState("Can Talk Bool?");

                foreach (var t in heroLookUp.Transitions) t.ToState = "Cancel";
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
                canTalkBool.AddFirstAction(new BoolTestMod(placement.AllObtained, "FALSE", null));
            }
        }

        public static string GetTabletName(AbstractPlacement placement)
        {
            return $"Lore Tablet-{placement.Name}";
        }


    }
}
