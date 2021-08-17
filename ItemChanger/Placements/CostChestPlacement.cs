using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using ItemChanger.FsmStateActions;
using UnityEngine;
using ItemChanger.Extensions;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using ItemChanger.Util;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class CostChestPlacement : MultiLocationPlacement, IContainerPlacement
    {
        public override AbstractLocation Location => chestLocation;
        public override IEnumerable<AbstractLocation> SecondaryLocations => 
            tabletLocation.Yield<AbstractLocation>();
        public ContainerLocation chestLocation;
        public PlaceableLocation tabletLocation;

        public void AddItem(AbstractItem item, Cost cost)
        {
            CostTag tag = item.GetTag<CostTag>() ?? item.AddTag<CostTag>();
            tag.Cost = cost;
            Items.Add(item);
        }

        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (location == chestLocation)
            {
                obj = ChestUtility.MakeNewChest(this);
                containerType = Container.Chest;
                EditChestFsm(obj);
            }
            else if (location == tabletLocation)
            {
                ItemChangerMod.instance.Log("Getting tablet");
                obj = TabletUtility.MakeNewTablet(this, BuildText);
                containerType = Container.Tablet;
            }
            else throw new ArgumentException($"Unknown location {location.name} found in GetContainer.");
        }

        public void EditChestFsm(GameObject chest)
        {
            PlayMakerFSM chestFsm = chest.LocateFSM("Chest Control");

            FsmState init = chestFsm.GetState("Init");
            FsmState spawnItems = chestFsm.GetState("Spawn Items");

            FsmStateAction checkAction = new Lambda(() => chestFsm.SendEvent(AllObtained() ? "ACTIVATE" : null));

            init.RemoveActionsOfType<BoolTest>();
            init.AddLastAction(checkAction);

            // Destroy any existing shinies in the chest
            // Moved to MakeNewChest, this code can likely be removed safely
            GameObject itemParent = chestFsm.gameObject.transform.Find("Item").gameObject;
            foreach (Transform t in itemParent.transform)
            {
                UnityEngine.Object.Destroy(t.gameObject);
            }

            // Remove pre-existing geo from chest
            foreach (FlingObjectsFromGlobalPool fling in spawnItems.GetActionsOfType<FlingObjectsFromGlobalPool>())
            {
                fling.spawnMin = 0;
                fling.spawnMax = 0;
            }

            // Need to check SpawnFromPool action too because of Mantis Lords chest
            foreach (SpawnFromPool spawn in spawnItems.GetActionsOfType<SpawnFromPool>())
            {
                spawn.spawnMin = 0;
                spawn.spawnMax = 0;
            }

            void OnOpenChest()
            {
                foreach (AbstractItem item in Items)
                {
                    Cost cost = item.GetTag<CostTag>()?.Cost;

                    if (!item.IsObtained())
                    {
                        if (cost != null && !cost.Paid && !cost.CanPay()) continue;
                        if (cost != null && !cost.Paid) cost.Pay();
                        if (item.GiveEarly(Container.Chest))
                        {
                            item.Give(this, new GiveInfo
                            {
                                Container = Container.Chest,
                                FlingType = chestLocation.flingType,
                                Transform = chestFsm.gameObject.transform,
                                MessageType = MessageType.Corner,
                            });
                        }
                        else
                        {
                            GameObject shiny = ShinyUtility.MakeNewShiny(this, item, chestLocation.flingType);
                            ShinyUtility.PutShinyInContainer(itemParent, shiny);
                            ShinyUtility.FlingShinyRandomly(shiny.LocateFSM("Shiny Control"));
                        }
                    }

                    foreach (Transform t in itemParent.transform) t.gameObject.SetActive(true);
                }
            }

            spawnItems.AddLastAction(new Lambda(OnOpenChest));
        }

        public string BuildText()
        {
            StringBuilder sb = new StringBuilder("Chest Contents<br>");
            for (int i = 0; i < Items.Count; i++)
            {
                AbstractItem item = Items[i];
                Cost cost = item.GetTag<CostTag>()?.Cost;

                sb.Append("<br>");
                sb.Append(item?.GetResolvedUIDef(this)?.GetPostviewName() ?? "Unknown Item");
                sb.Append("  -  ");
                if (item.IsObtained())
                {
                    sb.Append("Obtained");
                }
                else if (cost is null)
                {
                    sb.Append("Free");
                }
                else if (cost.Paid)
                {
                    sb.Append("Purchased");
                }
                else
                {
                    sb.Append(cost.GetCostText());
                }
            }
            return sb.ToString();
        }
    }
}
