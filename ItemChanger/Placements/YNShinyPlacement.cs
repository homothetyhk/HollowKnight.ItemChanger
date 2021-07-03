using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Locations;
using ItemChanger.Util;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class YNShinyPlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement
    {
        public ContainerLocation location;
        public override AbstractLocation Location => location;

        public Cost Cost { get; set; }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
        }

        public void AddItemWithCost(AbstractItem item, Cost cost)
        {
            Items.Add(item);
            this.Cost = cost;
        }

        public void GetPrimaryContainer(out GameObject obj, out Container container)
        {
            obj = ShinyUtility.MakeNewMultiItemShiny(this, Items);
            container = Container.Shiny;
        }
    }
}
