using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using ItemChanger.Components;
using ItemChanger.Internal;

namespace ItemChanger.Util
{
    public static class ChestUtility
    {
        public const float CHEST_ELEVATION = 0.5f;

        public static GameObject MakeNewChest(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GameObject chest = ObjectCache.Chest;
            chest.name = GetChestName(placement);

            // Resize colliders so that chest lands on ground -- orig is size (2.4, 2) with offset (0.1, -1.3)
            chest.transform.Find("Bouncer").GetComponent<BoxCollider2D>().size = chest.GetComponent<BoxCollider2D>().size = new Vector2(2.4f, 1.2f);
            chest.AddComponent<DropIntoPlace>();

            var info = chest.GetOrAddComponent<ContainerInfo>();
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };

            return chest;
        }

        public static string GetChestName(AbstractPlacement placement)
        {
            return $"Chest-{placement.Name}";
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
            chest.transform.position = new Vector3(pos.x, pos.y + CHEST_ELEVATION - elevation, 0);
            chest.SetActive(target.activeSelf);
            //chest.SetActive(true); // is this necessary?
        }

        public static void MoveChest(GameObject chest, float x, float y, float elevation)
        {
            chest.transform.position = new Vector3(x, y + CHEST_ELEVATION - elevation, chest.transform.position.z);
            chest.SetActive(true); // is this necessary?
        }

        public static void ModifyChest(PlayMakerFSM chestFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            FsmState init = chestFsm.GetState("Init");
            FsmState spawnItems = chestFsm.GetState("Spawn Items");

            FsmStateAction checkAction = new Lambda(() => chestFsm.SendEvent(placement.CheckVisited() ? "ACTIVATE" : null));

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
                    spawnItems.AddAction(new Lambda(() => item.Give(placement, info)));
                }
                else
                {
                    GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                    ShinyUtility.PutShinyInContainer(itemParent, shiny);
                    ShinyUtility.FlingShinyRandomly(shiny.LocateFSM("Shiny Control"));
                }
            }
        }
    }
}
