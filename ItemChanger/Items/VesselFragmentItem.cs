using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    public class VesselFragmentItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.vesselFragmentCollected), true);
            int count = PlayerData.instance.GetInt(nameof(PlayerData.vesselFragments)) + amount;
            for (; count > 2; count -= 3)
            {
                HeroController.instance.AddToMaxMPReserve(33);
                PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
            }
            switch (count)
            {
                case 0 when PlayerData.instance.GetInt(nameof(PlayerData.MPReserveMax)) == PlayerData.instance.GetInt(nameof(PlayerData.MPReserveCap)):
                    PlayerData.instance.SetInt(nameof(PlayerData.vesselFragments), 3);
                    break;
                default:
                    PlayerData.instance.SetInt(nameof(PlayerData.vesselFragments), count);
                    break;
            }
        }


    }
}
