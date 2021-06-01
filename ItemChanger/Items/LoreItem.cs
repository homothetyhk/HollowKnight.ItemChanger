using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    public class LoreItem : AbstractItem
    {
        public string loreSheet;
        public string loreKey;
        public TextType textType;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            if (container == Container.Shop) return;
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
