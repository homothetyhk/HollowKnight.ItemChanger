using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    /// <summary>
    /// The default placement for most use cases.
    /// Chooses an item container for its location based on its item list.
    /// By design, no default support for costs.
    /// </summary>
    public class MutablePlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement
    {
        public MutablePlacement(string Name) : base(Name) { }

        public ContainerLocation Location;

        public override string MainContainerType => containerType;
        public string containerType = Container.Unknown;

        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            Location.Placement = this;
            Location.Load();
            Events.AddFsmEdit(new("Shiny Control"), SetShinyFling);
        }

        protected override void OnUnload()
        {
            Location.Unload();
            Events.RemoveFsmEdit(new("Shiny Control"), SetShinyFling);
        }

        private void SetShinyFling(PlayMakerFSM fsm)
        {
            if (fsm.gameObject.name == ShinyUtility.GetShinyPrefix(this))
            {
                ShinyUtility.FlingShinyDown(fsm);
                fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
            }
            else if (ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out _))
            {
                switch (containerType)
                {
                    // Leave at location
                    case Container.Shiny:
                        ShinyUtility.FlingShinyDown(fsm);
                        fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
                        break;

                    // Fling from location
                    case Container.Chest:
                    case Container.GeoRock:
                    case Container.GrubJar:
                    default:
                        if (!CheckVisitedAny(VisitState.Opened) && Location.flingType == FlingType.Everywhere)
                        {
                            ShinyUtility.FlingShinyRandomly(fsm);
                        }
                        else
                        {
                            ShinyUtility.FlingShinyDown(fsm);
                        }
                        fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
                        break;
                }
            }
        }

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = ChooseContainerType(this, location as ContainerLocation, Items);
            }
            
            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (container == null || !container.SupportsInstantiate)
            {
                this.containerType = containerType = ChooseContainerType(this, location as ContainerLocation, Items);
                container = Container.GetContainer(containerType);
                if (container == null) throw new InvalidOperationException($"Unable to resolve container type {containerType} for placement {Name}!");
            }

            obj = container.GetNewContainer(this, Items, location.flingType, Cost);
        }

        public static string ChooseContainerType(ISingleCostPlacement placement, ContainerLocation location, IEnumerable<AbstractItem> items)
        {
            if (location?.forceShiny ?? true)
            {
                return Container.Shiny;
            }

            bool mustSupportCost = placement.Cost != null;

            string containerType = items
                .Select(i => i.GetPreferredContainer())
                .FirstOrDefault(c => Container.GetContainer(c) is Container container && container.SupportsInstantiate && location.Supports(c) && (!mustSupportCost || container.SupportsCost));

            if (string.IsNullOrEmpty(containerType))
            {
                if (mustSupportCost || items.Count() == 1) containerType = Container.Shiny;
                else containerType = Container.Chest;
            }

            return containerType;
        }
    }
}
