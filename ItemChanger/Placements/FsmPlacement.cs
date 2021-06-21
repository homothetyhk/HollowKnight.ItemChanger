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
    public class FsmPlacement : AbstractPlacement, IFsmLocationActions
    {
        public FsmLocation location;
        public override string SceneName => location.sceneName;

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            location.OnEnable(fsm, this);
        }

        public override string OnLanguageGet(string convo, string sheet)
        {
            return location.OnLanguageGet(convo, sheet, GetItemName);
        }


        public void Give(Action callback)
        {
            GiveInfo info = new GiveInfo
            {
                Container = Container.NPCDialogue,
                FlingType = location.flingType,
                Transform = location.FindTransformInScene(),
                MessageType = location.messageType,
            };

            ItemUtility.GiveSequentially(items, this, info, callback);
        }

        public bool AllObtained()
        {
            return items.All(i => i.IsObtained());
        }

        public string GetUIItemName(int maxLength = 120)
        {
            IEnumerable<string> itemNames = items.Where(i => !i.IsObtained()).Select(i => i.UIDef?.GetDisplayName() ?? "Unknown Item");
            string itemText = string.Join(", ", itemNames.ToArray());
            if (itemText.Length > maxLength) itemText = itemText.Substring(0, 117) + "...";
            return itemText;
        }

        public void SetVisited()
        {
            visited = true;
        }

        public bool CheckVisited()
        {
            return visited;
        }

        public string GetItemName()
        {
            string itemText = string.Join(", ", items.Select(i => i.UIDef.GetDisplayName()).ToArray());
            if (itemText.Length > 120) itemText = itemText.Substring(0, 117) + "...";
            return itemText;
        }
    }
}
