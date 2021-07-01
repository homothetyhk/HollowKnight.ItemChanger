using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Util;

namespace ItemChanger.Locations
{
    // TODO: Inherit ContainerLocation?
    public class ExistingChestLocation : AbstractLocation
    {
        public string chestFsm;
        public string chestName;

        public void PlaceInChest(PlayMakerFSM fsm, AbstractPlacement placement)
        {
            if (fsm.FsmName == chestFsm && fsm.gameObject.name == chestName)
            {
                ChestUtility.ModifyChest(fsm, flingType, placement, placement.Items);
            }

            if (fsm.FsmName == "Shiny Control" && ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, placement, out var item))
            {
                ShinyUtility.ModifyShiny(fsm, flingType, placement, item);
                if (!placement.CheckVisited() && flingType == FlingType.Everywhere)
                {
                    ShinyUtility.FlingShinyRandomly(fsm);
                }
                else
                {
                    ShinyUtility.FlingShinyDown(fsm);
                }
            }
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.ExistingChestPlacement
            {
                location = this,
            };
        }
    }
}
