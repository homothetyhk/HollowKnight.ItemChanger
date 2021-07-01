using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;
using ItemChanger.Util;
using ItemChanger.Locations;

namespace ItemChanger.Placements
{
    public class ExistingChestPlacement : AbstractPlacement
    {
        public ExistingChestLocation location;
        public override AbstractLocation Location => location;

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            base.OnEnableFsm(fsm);
            location.PlaceInChest(fsm, this);
        }
    }
}
