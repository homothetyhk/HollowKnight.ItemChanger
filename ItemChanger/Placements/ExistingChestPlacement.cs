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

namespace ItemChanger.Placements
{
    public class ExistingChestPlacement : AbstractPlacement
    {
        public string chestName;
        public string chestFsm;
        public override string SceneName => sceneName;
        public string sceneName;


        public FlingType flingType;

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == chestFsm && fsm.gameObject.name == chestName)
            {
                ChestUtility.ModifyChest(fsm, flingType, this, items);
            }

            if (fsm.FsmName == "Shiny Control" && ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out var item))
            {
                ShinyUtility.ModifyShiny(fsm, flingType, this, item);
                if (!HasVisited() && flingType == FlingType.Everywhere)
                {
                    ShinyUtility.FlingShinyRandomly(fsm);
                }
                else
                {
                    ShinyUtility.FlingShinyDown(fsm);
                }
            }
        }
    }
}
