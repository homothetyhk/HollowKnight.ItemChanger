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
    /// <summary>
    /// Variant of MutablePlacement which only uses the shiny container type.
    /// </summary>
    public class YNShinyPlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement, IPrimaryLocationPlacement
    {
        public YNShinyPlacement(string Name) : base(Name) { }

        public ContainerLocation Location;
        AbstractLocation IPrimaryLocationPlacement.Location => Location;

        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            Location.Placement = this;
            Location.Load();
        }

        protected override void OnUnload()
        {
            Location.Unload();
        }

        public void AddItemWithCost(AbstractItem item, Cost cost)
        {
            Items.Add(item);
            this.Cost = cost;
        }

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            obj = ShinyUtility.MakeNewMultiItemShiny(this, Items, Location.flingType, Cost);
            containerType = Container.Shiny;
        }
    }
}
