using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    public class SpawnLumafliesItem : AbstractItem
    {
        public override bool GiveEarly(string containerType) => containerType switch
        {
            Container.Chest 
            or Container.GrubJar
            or Container.GeoRock
            or Container.Enemy
            or Container.Mimic 
              => true,
            _ => false
        };

        public override void GiveImmediate(GiveInfo info)
        {
            Transform t = info.Transform != null ? info.Transform : HeroController.instance.transform;
            if (t == null) return;

            GameObject lumafly = Internal.ObjectCache.LumaflyEscape;
            lumafly.transform.position = t.position - 5 * Vector3.up;
            lumafly.SetActive(true);
        }
    }
}
