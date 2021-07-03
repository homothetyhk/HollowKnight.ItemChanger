using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Util;
using SereCore;
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

        public Container container = Container.Shiny;

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
                    switch (container)
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

        public void GetPrimaryContainer(out GameObject obj, out Container container)
        {
            SetContainerType();
            container = this.container;
            obj = ContainerUtility.GetNewContainer(this, Items, container);
        }

        public void SetContainerType()
        {
            if (!location.forceShiny && container == Container.Shiny)
            {
                container = Items.Select(i => i.GetPreferredContainer()).FirstOrDefault(c => c != Container.Shiny && location.Supports(c));
                if (container == Container.Shiny && location.Supports(Container.Chest) && Items.Count() > 1) container = Container.Chest;
            }
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
