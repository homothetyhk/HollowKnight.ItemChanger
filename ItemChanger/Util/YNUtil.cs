using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class YNUtil
    {
        static GameObject DialogueManager => FsmVariables.GlobalVariables.FindFsmGameObject("DialogueManager").Value;

        public static void OpenYNDialogue(GameObject requester, IEnumerable<AbstractItem> items, Cost cost)
        {
            string text = string.Join(", ", items.Where(i => !i.IsObtained()).Select(i => i.UIDef.GetPreviewName()).ToArray());
            if (text.Length > 120)
            {
                text = text.Substring(0, 117) + "...";
            }

            if (cost == null || cost.Paid)
            {
                OpenYNDialogue(requester, text, true);
            }
            else if (cost is GeoCost gc)
            {
                OpenYNDialogue(requester, text, gc.amount);
            }
            else
            {
                string costText = cost.GetCostText();
                if (costText.Length > 40)
                {
                    costText = costText.Substring(0, 37) + "...";
                }
                OpenYNDialogue(requester, $"{text}\n{costText}", cost.CanPay());
            }
        }

        public static void OpenYNDialogue(GameObject requester, string text, bool canPay)
        {
            GameObject dm = DialogueManager;
            GameObject yn = dm.transform.Find("Text YN").gameObject;
            PlayMakerFSM dpc = FSMUtility.LocateFSM(yn, "Dialogue Page Control");

            FSMUtility.LocateFSM(dm, "Box Open YN").SendEvent("BOX UP YN");

            dpc.FsmVariables.GetFsmGameObject("Requester").Value = requester;

            if (!canPay)
            {
                dpc.StartCoroutine(YNUtil.KillGeoText());
            }

            dpc.FsmVariables.GetFsmInt("Toll Cost").Value = 0;
            dpc.FsmVariables.GetFsmGameObject("Geo Text").Value.SetActive(true);

            Internal.DialogueCenter.StartConversationYN(text);
        }

        public static void OpenYNDialogue(GameObject requester, string text, int geoAmount)
        {
            GameObject dm = DialogueManager;
            GameObject yn = dm.transform.Find("Text YN").gameObject;
            PlayMakerFSM dpc = FSMUtility.LocateFSM(yn, "Dialogue Page Control");

            FSMUtility.LocateFSM(dm, "Box Open YN").SendEvent("BOX UP YN");

            dpc.FsmVariables.GetFsmGameObject("Requester").Value = requester;
            dpc.FsmVariables.GetFsmInt("Toll Cost").Value = geoAmount;
            dpc.FsmVariables.GetFsmGameObject("Geo Text").Value.SetActive(true);
            dpc.StartCoroutine(EraseTollCost()); // sets the toll cost back to 0 after it sets the text but before it subtracts geo. Useful when the request comes from a GeoCost.

            text = text.Length < 60 ? text : text.Substring(0, 57) + "...";
            Internal.DialogueCenter.StartConversationYN(text);
        }


        public static void CloseYNDialogue()
        {
            FSMUtility.LocateFSM(DialogueManager, "Box Open YN").SendEvent("BOX DOWN YN");
        }

        public static IEnumerator EraseTollCost()
        {
            PlayMakerFSM ynFsm = FSMUtility.LocateFSM(DialogueManager.transform.Find("Text YN").gameObject, "Dialogue Page Control");
            while (ynFsm.ActiveStateName != "Ready for Input")
            {
                yield return new WaitForEndOfFrame();
            }

            ynFsm.FsmVariables.GetFsmInt("Toll Cost").Value = 0;
        }

        public static IEnumerator KillGeoText()
        {
            PlayMakerFSM ynFsm = FSMUtility.LocateFSM(DialogueManager.transform.Find("Text YN").gameObject, "Dialogue Page Control");
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
