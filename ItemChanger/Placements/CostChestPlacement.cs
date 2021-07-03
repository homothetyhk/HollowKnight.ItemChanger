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
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class CostChestPlacement : AbstractPlacement, IContainerPlacement
    {
        public override AbstractLocation Location => chestLocation;
        public ContainerLocation chestLocation;
        public PlaceableLocation tabletLocation;

        public void AddItem(AbstractItem item, Cost cost)
        {
            CostTag tag = item.GetTag<CostTag>() ?? item.AddTag<CostTag>();
            tag.Cost = cost;
            Items.Add(item);
        }

        public void GetPrimaryContainer(out GameObject obj, out Container containerType)
        {
            obj = ChestUtility.MakeNewChest(this, Items);
            containerType = Container.Chest;
        }

        public override void OnLoad()
        {
            tabletLocation.auxillary = true;
            base.OnLoad();
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            base.OnActiveSceneChanged(from, to);
            tabletLocation.PlaceContainer(TabletUtility.MakeNewTablet(this), Container.Tablet);
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);

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

                FsmStateAction checkAction = new Lambda(() => fsm.SendEvent(AllObtained() ? "ACTIVATE" : null));

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
                    for (int i = 0; i < Items.Count; i++)
                    {
                        AbstractItem item = Items[i];
                        Cost cost = item.GetTag<CostTag>()?.Cost;

                        if (!item.IsObtained())
                        {
                            if (cost != null && !cost.Paid&& !cost.CanPay()) continue;
                            if (cost != null && !cost.Paid) cost.Pay();
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

        public override string OnLanguageGet(string convo, string sheet)
        {
            if (sheet == "ItemChanger.Locations" && convo == TabletUtility.GetTabletName(this))
            {
                StringBuilder sb = new StringBuilder("Chest Contents<br>");
                for (int i = 0; i < Items.Count; i++)
                {
                    AbstractItem item = Items[i];
                    Cost cost = item.GetTag<CostTag>()?.Cost;

                    sb.Append("<br>");
                    sb.Append(item.UIDef?.GetPostviewName() ?? "Unknown Item");
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
            return base.OnLanguageGet(convo, sheet);
        }
    }
}
