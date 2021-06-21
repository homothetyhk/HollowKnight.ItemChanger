using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;

namespace ItemChanger.Util
{
    public static class ChestUtility
    {
        public const float CHEST_ELEVATION = 0.5f;

        public static GameObject MakeNewChest(AbstractPlacement location)
        {
            GameObject chest = ObjectCache.Chest;
            chest.name = GetChestName(location);
            return chest;
        }

        public static string GetChestName(AbstractPlacement location)
        {
            return $"Chest-{location.name}";
        }

        public static void MoveChest(GameObject chest, GameObject target, float elevation)
        {
            if (target.transform.parent != null)
            {
                chest.transform.SetParent(target.transform.parent);
            }

            chest.transform.position = target.transform.position;
            chest.transform.localPosition = target.transform.localPosition;
            var pos = chest.transform.position;
            // Move the chest forward so it appears in front of any background objects
            chest.transform.position = new Vector3(pos.x, pos.y + CHEST_ELEVATION - elevation, pos.z);
            chest.SetActive(true); // is this necessary?
        }

        public static void MoveChest(GameObject chest, float x, float y, float elevation)
        {
            chest.transform.position = new Vector3(x, y + CHEST_ELEVATION - elevation, chest.transform.position.z);
            chest.SetActive(true); // is this necessary?
        }

        public static void ModifyChest(PlayMakerFSM chestFsm, FlingType flingType, AbstractPlacement location, IEnumerable<AbstractItem> items)
        {
            FsmState init = chestFsm.GetState("Init");
            FsmState spawnItems = chestFsm.GetState("Spawn Items");

            FsmStateAction checkAction = new Lambda(() => chestFsm.SendEvent(location.HasVisited() ? "ACTIVATE" : null));

            init.RemoveActionsOfType<BoolTest>();
            init.AddAction(checkAction);

            // Destroy any existing shinies in the chest
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

            foreach (AbstractItem item in items)
            {
                if (item.GiveEarly(Container.Chest))
                {
                    GiveInfo info = new GiveInfo
                    {
                        Container = Container.Chest,
                        FlingType = flingType,
                        Transform = chestFsm.transform,
                        MessageType = MessageType.Corner,
                    };
                    spawnItems.AddAction(new Lambda(() => item.Give(location, info)));
                }
                else
                {
                    GameObject shiny = ShinyUtility.MakeNewShiny(location, item);
                    ShinyUtility.PutShinyInContainer(itemParent, shiny);
                }
            }
        }
    }
}
