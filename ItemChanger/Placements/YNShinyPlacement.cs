using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Locations;
using ItemChanger.Util;

namespace ItemChanger.Placements
{
    public class YNShinyPlacement : AbstractPlacement
    {
        public IMutableLocation location;
        public Cost cost;
        public override string SceneName => location.sceneName;

        public override void OnActiveSceneChanged()
        {
            GameObject shiny = ShinyUtility.MakeNewMultiItemShiny(this);
            location.PlaceContainer(shiny, Container.Shiny);
        }

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Shiny Control" && fsm.gameObject.name == ShinyUtility.GetShinyPrefix(this))
            {
                ShinyUtility.ModifyMultiShiny(fsm, location.flingType, this, items);
                ShinyUtility.AddYNDialogueToShiny(fsm, cost, items);
            }
        }

        public void AddItemWithCost(AbstractItem item, Cost cost)
        {
            items.Add(item);
            this.cost = cost;
        }
    }
}
