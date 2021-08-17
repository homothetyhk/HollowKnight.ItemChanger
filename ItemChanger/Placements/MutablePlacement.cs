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
    public class MutablePlacement : AbstractPlacement, IContainerPlacement
    {
        public ContainerLocation location;
        public override AbstractLocation Location => location;

        public override string MainContainerType => containerType;
        public string containerType = Container.Unknown;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            switch (fsm.FsmName)
            {
                // Multi Shiny
                case "Shiny Control" when ShinyUtility.GetShinyPrefix(this) == fsm.gameObject.name:
                    ShinyUtility.FlingShinyDown(fsm);
                    fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
                    break;
                // Shiny
                case "Shiny Control" when ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out _):
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
                            if (!CheckVisitedAny(VisitState.Opened) && location.flingType == FlingType.Everywhere)
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
                    break;
                    /*
                    // Geo Rock
                    case "Geo Rock" when fsm.gameObject.name == GeoRockUtility.GetGeoRockName(this):
                        GeoRockUtility.ModifyGeoRock(fsm, location.flingType, this, Items);
                        break;

                    // Grub Jar
                    case "Bottle Control" when fsm.gameObject.name == GrubJarUtility.GetGrubJarName(this):
                        GrubJarUtility.ModifyBottleFsm(fsm, location.flingType, this, Items);
                        break;

                    // Chest
                    case "Chest Control" when fsm.gameObject.name == ChestUtility.GetChestName(this):
                        ChestUtility.ModifyChest(fsm, location.flingType, this, Items);
                        break;

                    // Multi Shiny
                    case "Shiny Control" when ShinyUtility.GetShinyPrefix(this) == fsm.gameObject.name:
                        ShinyUtility.ModifyMultiShiny(fsm, location.flingType, this, Items);
                        ShinyUtility.FlingShinyDown(fsm);
                        fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
                        break;

                    // Shiny
                    case "Shiny Control" when ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out item):
                        switch (container)
                        {
                            // Leave at location
                            case Container.Shiny:
                                ShinyUtility.ModifyShiny(fsm, location.flingType, this, item);
                                ShinyUtility.FlingShinyDown(fsm);
                                fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
                                break;

                            // Fling from location
                            case Container.Chest:
                            case Container.GeoRock:
                            case Container.GrubJar:
                            default:
                                ShinyUtility.ModifyShiny(fsm, location.flingType, this, item);
                                if (!CheckVisited() && location.flingType == FlingType.Everywhere)
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
                        break;
                    */
            }
        }

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = ChooseContainerType(location as ContainerLocation, Items);
            }
            
            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (containerType == null)
            {
                ItemChangerMod.instance.LogError($"Unknown container type {containerType} used for {Name}!");
            }

            obj = container.GetNewContainer(this, Items, location.flingType);
        }

        public static string ChooseContainerType(ContainerLocation location, IEnumerable<AbstractItem> items)
        {
            if (location?.forceShiny ?? true)
            {
                return Container.Shiny;
            }

            string containerType = items
                .Select(i => i.GetPreferredContainer())
                .FirstOrDefault(c => c != Container.Unknown && location.Supports(c));

            if (string.IsNullOrEmpty(containerType))
            {
                containerType = items.Count() == 1 ? Container.Shiny : Container.Chest;
            }

            return containerType;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            location.OnLoad();
        }

        public override void OnUnload()
        {
            base.OnUnload();
            location.OnUnload();
        }
    }
}
