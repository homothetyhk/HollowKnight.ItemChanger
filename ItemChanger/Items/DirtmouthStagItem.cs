using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Extensions;

namespace ItemChanger.Items
{
    public class DirtmouthStagItem : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.openedTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.openedTownBuilding), true);
            if (GameManager.instance.sceneName == "Room_Town_Stag_Station")
            {
                UnityEngine.GameObject.Find("Station Door").LocateFSM("Control").SendEvent("ACTIVATE");
            }
        }

        // AlreadyObtained may conflict with transition fixes?
    }
}
