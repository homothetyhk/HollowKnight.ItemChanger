using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.Items
{
    public class MimicItem : AbstractItem
    {
        public override string GetPreferredContainer()
        {
            return Container.Mimic;
        }

        public override bool GiveEarly(string containerType)
        {
            return containerType == Container.Mimic;
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.Container != Container.Mimic)
            {
                AudioSource.PlayClipAtPoint(ObjectCache.MimicScream,
                        new Vector3(
                            Camera.main.transform.position.x - 2,
                            Camera.main.transform.position.y,
                            Camera.main.transform.position.z + 2
                        ));
                AudioSource.PlayClipAtPoint(ObjectCache.MimicScream,
                    new Vector3(
                        Camera.main.transform.position.x + 2,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z + 2
                    ));
            }
        }
    }
}
