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
            foreach (AbstractItem item in items.Where(i => !i.IsObtained()))
            {
                item.Give(this, Container.Unknown, FlingType.DirectDeposit, null, MessageType.Corner);
            }
        }

    }
}
