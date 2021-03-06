﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using SereCore;
using TMPro;
using UnityEngine;

namespace ItemChanger.Internal
{
    public static class DialogueCenter
    {
        static GameObject DialogueManager => FsmVariables.GlobalVariables.FindFsmGameObject("DialogueManager").Value;
        static PlayMakerFSM BoxOpenFsm => DialogueManager.LocateFSM("Box Open");
        static GameObject DialogueText => FsmVariables.GlobalVariables.FindFsmGameObject("DialogueText").Value;
        static GameObject Arrow => DialogueText.transform.parent.Find("Arrow").gameObject;
        static GameObject Stop => DialogueText.transform.parent.Find("Stop").gameObject;

        static DialogueBox DialogueBox => DialogueText.GetComponent<DialogueBox>();
        static DialogueBox DialogueBoxYN => null;

        public static void SendLoreMessage(string text, Action callback, TextType type)
        {
            switch (type)
            {
                case TextType.LeftLore:
                    DialogueBox.StartCoroutine(LeftLoreCoroutine(text, callback));
                    break;
                case TextType.Lore:
                    DialogueBox.StartCoroutine(LoreCoroutine(text, callback));
                    break;
                case TextType.MajorLore:
                    DialogueBox.StartCoroutine(MajorLoreCoroutine(text, callback));
                    break;
            }
        }

        public static IEnumerator MajorLoreCoroutine(string text, Action callback)
        {
            PlayMakerFSM.BroadcastEvent("LORE PROMPT UP");
            PlayLoreSound();
            yield return new WaitForSeconds(0.85f);

            DialogueText.LocateFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = null;
            DialogueText.transform.localPosition = new Vector3(0, 2.44f, 0);
            Stop.transform.localPosition = new Vector3(0, -0.23f, 0);
            Arrow.transform.localPosition = new Vector3(0, -0.3f, 0);
            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            PlayMakerFSM.BroadcastEvent("LORE PROMPT DOWN");
            yield return new WaitForSeconds(0.5f);

            callback?.Invoke();

            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
            DialogueText.transform.localPosition = new Vector3(0, 4.49f, 0);
            Stop.transform.localPosition = new Vector3(0, 1.695f, 0);
            Arrow.transform.localPosition = new Vector3(0, 1.695f, 0);
        }

        public static IEnumerator LoreCoroutine(string text, Action callback)
        {
            BoxOpenFsm.Fsm.Event("BOX UP");
            yield return new WaitForSeconds(0.3f);

            DialogueText.LocateFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = null;
            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            BoxOpenFsm.Fsm.Event("BOX DOWN");
            yield return new WaitForSeconds(0.5f);

            callback?.Invoke();

            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
        }

        public static IEnumerator LeftLoreCoroutine(string text, Action callback)
        {
            BoxOpenFsm.Fsm.Event("BOX UP");
            yield return new WaitForSeconds(0.3f);

            DialogueText.LocateFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = null;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            BoxOpenFsm.Fsm.Event("BOX DOWN");
            yield return new WaitForSeconds(0.5f);

            callback?.Invoke();
        }

        public static void PlayLoreSound()
        {
            AudioSource.PlayClipAtPoint(ObjectCache.LoreSound,
                new Vector3(
                    Camera.main.transform.position.x - 2,
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z + 2
                ));
            AudioSource.PlayClipAtPoint(ObjectCache.LoreSound,
                new Vector3(
                    Camera.main.transform.position.x + 2,
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z + 2
                ));
        }

        public static void StartConversation(string text)
        {
            DialogueBox box = DialogueBox;
            box.currentPage = 1;
            TextMeshPro textMesh = box.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.ForceMeshUpdate();
            box.ShowPage(1);
        }

        public static void StartConversationYN(string text)
        {
            DialogueBox box = DialogueBoxYN;
            box.currentPage = 1;
            TextMeshPro textMesh = box.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.ForceMeshUpdate();
            box.ShowPage(1);
        }

        public static void Hook()
        {
            On.DialogueBox.HideText += HideTextListener;
        }

        private static void HideTextListener(On.DialogueBox.orig_HideText orig, DialogueBox self)
        {
            convoEnded = true;
            orig(self);
        }
        static bool ConvoEnded() => convoEnded;
        static bool convoEnded = false;
    }
}
