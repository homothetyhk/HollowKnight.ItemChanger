using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using ItemChanger.FsmStateActions;
using UnityEngine;
using SereCore;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using ItemChanger.Util;

namespace ItemChanger.Placements
{
    public class CostChestPlacement : AbstractPlacement
    {
        public List<Cost> costs = new List<Cost>();
        public IMutableLocation chestLocation;
        public IMutableLocation tabletLocation;
        public override string SceneName => chestLocation.sceneName;

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            RepairCosts();

            if (fsm.FsmName == "Inspection" && fsm.gameObject.name == TabletUtility.GetTabletName(this))
            {
                fsm.FsmVariables.FindFsmString("Convo Name").Value = fsm.gameObject.name;
                fsm.FsmVariables.FindFsmString("Sheet Name").Value = "ItemChanger.Locations";
            }

            if (fsm.FsmName == "Shiny Control" && ShinyUtility.TryGetItemFromShinyName(fsm.gameObject.name, this, out var shinyItem))
            {
                ShinyUtility.ModifyShiny(fsm, chestLocation.flingType, this, shinyItem);
                if (chestLocation.flingType == FlingType.Everywhere)
                {
                    ShinyUtility.FlingShinyRandomly(fsm);
                }
                else
                {
                    ShinyUtility.FlingShinyDown(fsm);
                }
            }

            if (fsm.FsmName == "Chest Control" && fsm.gameObject.name == ChestUtility.GetChestName(this))
            {
                FsmState init = fsm.GetState("Init");
                FsmState spawnItems = fsm.GetState("Spawn Items");

                FsmStateAction checkAction = new Lambda(() => fsm.SendEvent(items.All(i => i.IsObtained()) ? "ACTIVATE" : null));

                init.RemoveActionsOfType<BoolTest>();
                init.AddAction(checkAction);

                // Destroy any existing shinies in the chest
                GameObject itemParent = fsm.gameObject.transform.Find("Item").gameObject;
                foreach (Transform t in itemParent.transform)
                {
                    GameObject.Destroy(t.gameObject);
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

                FsmStateAction generateItems = new Lambda(() =>
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        var cost = costs[i];

                        if (!item.IsObtained())
                        {
                            if (cost != null && !cost.Paid() && !cost.CanPay()) continue;
                            if (cost != null && !cost.Paid()) cost.Pay();
                            if (item.GiveEarly(Container.Chest))
                            {
                                item.Give(this, new GiveInfo
                                {
                                    Container = Container.Chest,
                                    FlingType = chestLocation.flingType,
                                    Transform = fsm.gameObject.transform,
                                    MessageType = MessageType.Corner,
                                });
                            }
                            else
                            {
                                GameObject shiny = ShinyUtility.MakeNewShiny(this, item);
                                ShinyUtility.PutShinyInContainer(itemParent, shiny);
                            }
                        }
                    }
                });

                fsm.GetState("Open").AddAction(generateItems);
            }
        }

        public override void OnActiveSceneChanged()
        {
            chestLocation.PlaceContainer(ChestUtility.MakeNewChest(this), Container.Chest);
            GameObject tablet = TabletUtility.MakeNewTablet(this);
            tabletLocation.PlaceContainer(tablet, Container.Tablet);
        }

        public override string OnLanguageGet(string convoName, string sheet)
        {
            if (sheet == "ItemChanger.Locations" && convoName == TabletUtility.GetTabletName(this))
            {
                StringBuilder sb = new StringBuilder("Chest Contents<br>");
                for (int i = 0; i < items.Count; i++)
                {
                    sb.Append("<br>");
                    sb.Append(items[i].UIDef?.GetDisplayName() ?? "Unknown Item");
                    sb.Append("  -  ");
                    if (items[i].IsObtained())
                    {
                        sb.Append("Obtained");
                    }
                    else if (costs[i] is null)
                    {
                        sb.Append("Free");
                    }
                    else if (costs[i].Paid())
                    {
                        sb.Append("Purchased");
                    }
                    else
                    {
                        sb.Append(costs[i].GetCostText());
                    }
                }
                return sb.ToString();
            }
            return null;
        }

        public override void AddItem(AbstractItem item)
        {
            RepairCosts();
            base.AddItem(item);
            costs.Add(null);
        }

        public void RepairCosts()
        {
            if (costs.Count < items.Count) costs.AddRange(Enumerable.Repeat<Cost>(null, items.Count - costs.Count));
        }

        public void AddItemWithCost(AbstractItem item, Cost cost)
        {
            RepairCosts();
            items.Add(item);
            costs.Add(cost);
        }

    }
}
