using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SereCore;

namespace ItemChanger.Items
{
    public class DirtmouthStagItem : AbstractItem
    {
        public override void GiveImmediate(Container container, FlingType fling, UnityEngine.Transform transform)
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
