using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives a grub and requests a Grub Jar container.
    /// </summary>
    public class GrubItem : AbstractItem
    {
        public override string GetPreferredContainer() => Container.GrubJar;
        public override bool GiveEarly(string containerType)
        {
            return containerType == Container.GrubJar;
        }
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.IncrementInt(nameof(PlayerData.grubsCollected));
            if (info.Container == Container.GrubJar) return;

            SoundManager.PlayClipAtPoint(SoundManager.RandomGrubCry, 
                info.Transform != null ? info.Transform.position
                : HeroController.instance != null ? HeroController.instance.transform.position
                : Camera.main.transform.position + 2 * Vector3.up);
        }
    }
}
