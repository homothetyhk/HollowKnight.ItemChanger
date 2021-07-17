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

    }
}
