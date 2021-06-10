using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class FsmPlacement : AbstractPlacement
    {
        public FsmLocation location;
        public override string SceneName => location.sceneName;

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            location.OnEnable(fsm, Check, Give);
        }

        public override string OnLanguageGet(string convo, string sheet)
        {
            if (location.CallLanguageHook(convo, sheet))
            {
                string itemText = string.Join(", ", items.Select(i => i.UIDef.GetDisplayName()).ToArray());
                if (itemText.Length > 120) itemText = itemText.Substring(0, 117) + "...";

                return location.OnLanguageGet(convo, sheet, itemText);
            }

            return null;
        }

        public bool Check()
        {
            return items.All(i => i.IsObtained());
        }

        public void Give(Action callback)
        {
            ItemUtility.GiveSequentially(items, this, Container.NPCDialogue, location.flingType, location.FindTransformInScene(), location.messageType, callback);
        }
    }
}
