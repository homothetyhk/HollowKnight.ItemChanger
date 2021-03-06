﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Internal
{
    public class MessageController : MonoBehaviour
    {
        private static MessageController instance;
        private static Coroutine activeDisplay;
        private readonly Queue<(Sprite sprite, string text)> messages = new Queue<(Sprite icon, string text)>();

        public void Update()
        {
            if (messages.Any() && activeDisplay == null)
            {
                var (sprite, text) = messages.Dequeue();
                activeDisplay = StartCoroutine(SendMessage(sprite, text));
            }
        }

        public static void Enqueue(Sprite sprite, string text)
        {
            instance.messages.Enqueue((sprite, text));
        }

        private IEnumerator SendMessage(Sprite sprite, string text)
        {
            GameObject popup = ObjectCache.RelicGetMsg;
            popup.transform.Find("Text").GetComponent<TMPro.TextMeshPro>().text = text;
            popup.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = sprite;
            popup.SetActive(true);
            yield return new WaitForSeconds(3f);
            activeDisplay = null;
        }


        internal static void Setup()
        {
            GameObject obj = new GameObject("MessageController parent");
            instance = obj.AddComponent<MessageController>();
            DontDestroyOnLoad(obj);
        }
    }
}
