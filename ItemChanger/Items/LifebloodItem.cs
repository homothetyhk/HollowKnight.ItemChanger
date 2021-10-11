using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives the specified amount of blue health.
    /// </summary>
    public class LifebloodItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            try
            {
                PlayMakerFSM blueHealthControl = PlayMakerFSM.FsmList.FirstOrDefault(f => f != null && f.FsmName == "Blue Health Control");
                if (blueHealthControl != null) blueHealthControl.SendEvent("INVENTORY OPENED");
            }
            catch (Exception) { }

            for (int i = 0; i < amount; i++)
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
            }
        }
    }
}
