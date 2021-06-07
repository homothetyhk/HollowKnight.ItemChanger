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
    public class MutablePlacement : AbstractPlacement
    {
        public IMutableLocation location;
        public Container container = Container.Shiny;
        public override string SceneName => location.sceneName;
        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            AbstractItem item;
            switch (fsm.FsmName)
            {
                // Geo Rock
                case "Geo Rock" when fsm.gameObject.name == GeoRockUtility.GetGeoRockName(this):
                    GeoRockUtility.ModifyGeoRock(fsm, location.flingType, this, items);
                    break;

                // Grub Jar
                case "Bottle Control" when fsm.gameObject.name == GrubJarUtility.GetGrubJarName(this):
                    GrubJarUtility.ModifyBottleFsm(fsm.gameObject, location.flingType, this, items);
                    break;

                // Chest
                case "Chest Control" when fsm.gameObject.name == ChestUtility.GetChestName(this):
                    ChestUtility.ModifyChest(fsm, location.flingType, this, items);
                    break;

                // Multi Shiny
                case "Shiny Control" when ShinyUtility.GetShinyPrefix(this) == fsm.gameObject.name:
                    ShinyUtility.ModifyMultiShiny(fsm, location.flingType, this, items);
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
                            if (!HasVisited() && location.flingType == FlingType.Everywhere)
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
            }
        }

        public override void OnActiveSceneChanged()
        {
            if (!location.forceShiny && container == Container.Shiny)
            {
                container = items.Select(i => i.GetPreferredContainer()).FirstOrDefault(c => c != Container.Shiny && location.Supports(c));
                if (container == Container.Shiny && location.Supports(Container.Chest) && items.Count > 1) container = Container.Chest;
            }
            GameObject obj = ContainerUtility.GetNewContainer(this, items, container);
            location.PlaceContainer(obj, container);
        }
    }
}
