using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class AddGeoItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
        {
            HeroController.instance.AddGeo(amount);
        }
    }
}
