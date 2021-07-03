using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine.SceneManagement;
using TMPro;

namespace ItemChanger.Components
{
    public class HintBox : MonoBehaviour
    {
        public static HintBox Create(Vector2 pos, Vector2 size)
        {
            GameObject obj = new GameObject("Hint Box");
            obj.transform.position = pos;
            BoxCollider2D box = obj.AddComponent<BoxCollider2D>();
            box.size = size;
            box.isTrigger = true;

            HintBox hint = obj.AddComponent<HintBox>();

            return hint;
        }

        public Func<string> GetDisplayText;
        PlayMakerFSM display;

        public void Setup(GameObject prefab)
        {
            display = GameObject.Instantiate(prefab).LocateMyFSM("Display");
            display.GetState("Init").RemoveActionsOfType<SetGameObject>();
            display.GetState("Check Convo").RemoveActionsOfType<StringCompare>();
            display.GetState("Set Convo").Actions = new FsmStateAction[0];
        }

        public void Update()
        {
            if (display == null) // Succeeds on the second update
            {
                var g = FsmVariables.GlobalVariables.FindFsmGameObject("Enemy Dream Msg");
                if (g.Value != null) Setup(g.Value);
            }
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && display != null)
            {
                string s = GetDisplayText();
                if (!string.IsNullOrEmpty(s))
                {
                    SetText(s);
                    ShowConvo();
                }
            }
        }

        private void SetText(string text)
        {
            display.FsmVariables.FindFsmGameObject("Text").Value.GetComponent<TextMeshPro>().text = text;
        }

        private void ShowConvo()
        {
            display.SendEvent("DISPLAY ENEMY DREAM");
        }
    }
}
