﻿using System;
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
            for (int i = 0; i < amount; i++)
            {
                GiveVesselFragment();
            }
        }

        public static void GiveVesselFragment()
        {
            PlayerData.instance.SetBool(nameof(PlayerData.vesselFragmentCollected), true);
            PlayerData.instance.IncrementInt(nameof(PlayerData.vesselFragments));

            if (PlayerData.instance.GetInt(nameof(PlayerData.vesselFragments)) >= 3)
            {
                HeroController.instance.AddToMaxMPReserve(33);
                PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
            }

            if (PlayerData.instance.GetInt(nameof(PlayerData.MPReserveMax)) == PlayerData.instance.GetInt(nameof(PlayerData.MPReserveCap)))
            {
                PlayerData.instance.SetInt(nameof(PlayerData.vesselFragments), 3);
            }
            else PlayerData.instance.IntAdd(nameof(PlayerData.vesselFragments), -3);
        }
    }
}
