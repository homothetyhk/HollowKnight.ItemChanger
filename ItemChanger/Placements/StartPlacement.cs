using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Placements
{
    public class StartPlacement : AbstractPlacement
    {
        public override string SceneName => null;
        
        public void GiveRemainingItems()
        {
            GiveInfo info = new GiveInfo
            {
                Container = Container.Unknown,
                FlingType = FlingType.DirectDeposit,
                Transform = null,
                MessageType = MessageType.Corner,
            };
            foreach (AbstractItem item in items.Where(i => !i.IsObtained()))
            {
                item.Give(this, info);
            }
        }

    }
}
