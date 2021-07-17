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

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            obj = ShinyUtility.MakeNewMultiItemShiny(this, Items, Location.flingType);
            containerType = Container.Shiny;
        }
    }
}
