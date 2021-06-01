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
        public override string SceneName => location.sceneName;
        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Geo Rock" && fsm.gameObject.name == GeoRockUtility.GetGeoRockName(this))
            {
                GeoRockUtility.ModifyGeoRock(fsm, location.flingType, this, items);
            }

            if (fsm.FsmName == "Shiny Control" && ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out var item))
            {
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
            }

            if (fsm.FsmName == "Shiny Control" && ShinyUtility.GetShinyPrefix(this) == fsm.gameObject.name)
            {
                ShinyUtility.ModifyMultiShiny(fsm, location.flingType, this, items);
                if (!HasVisited() && location.flingType == FlingType.Everywhere)
                {
                    ShinyUtility.FlingShinyRandomly(fsm);
                }
                else
                {
                    ShinyUtility.FlingShinyDown(fsm);
                }
                fsm.gameObject.transform.Translate(new Vector3(0, 0, -0.1f));
            }

            if (fsm.FsmName == "Bottle Control" && fsm.gameObject.name == GrubJarUtility.GetGrubJarName(this))
            {
                GrubJarUtility.ModifyBottleFsm(fsm.gameObject, location.flingType, this, items);
            }

            if (fsm.FsmName == "Chest Control" && fsm.gameObject.name == ChestUtility.GetChestName(this))
            {
                ChestUtility.ModifyChest(fsm, location.flingType, this, items);
            }
        }

        public override void OnActiveSceneChanged()
        {
            Container container = Container.Shiny;
            if (!location.forceShiny)
            {
                container = items.Select(i => i.GetPreferredContainer()).FirstOrDefault(c => c != Container.Shiny && location.Supports(c));
                if (container == Container.Shiny && location.Supports(Container.Chest) && items.Count > 1) container = Container.Chest;
            }
            GameObject obj = ContainerUtility.GetNewContainer(this, items, container);
            location.PlaceContainer(obj, container);
        }
    }
}
