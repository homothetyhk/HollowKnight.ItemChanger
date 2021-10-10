using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.Items
{
    public class LoreItem : AbstractItem
    {
        public string loreSheet;
        public string loreKey;
        public TextType textType;

        public override void GiveImmediate(GiveInfo info)
        {
            if ((info.MessageType & MessageType.Lore) == MessageType.Lore) return;
            SoundManager.PlayClipAtPoint(SoundManager.LoreSound,
                info.Transform != null ? info.Transform.position
                : HeroController.instance != null ? HeroController.instance.transform.position
                : Camera.main.transform.position + 2 * Vector3.up);
        }

    }
}
