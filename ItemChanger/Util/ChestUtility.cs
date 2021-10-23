using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;
using ItemChanger.Components;
using ItemChanger.Internal;

namespace ItemChanger.Util
{
    public static class ChestUtility
    {
        public const float CHEST_ELEVATION = 0.5f;

        public static GameObject MakeNewChest(AbstractPlacement placement)
        {
            GameObject chest = ObjectCache.Chest;
            chest.name = GetChestName(placement);

            // prevent collision with corpses, etc
            chest.layer = 0;
            chest.transform.Find("Opened").gameObject.layer = 0;

            // Resize colliders so that chest lands on ground -- orig is size (2.4, 2) with offset (0.1, -1.3)
            chest.transform.Find("Bouncer").GetComponent<BoxCollider2D>().size = chest.GetComponent<BoxCollider2D>().size = new Vector2(2.4f, 1.2f);
            chest.AddComponent<DropIntoPlace>().OnLand += () =>
            {
                chest.layer = 8;
                chest.transform.Find("Opened").gameObject.layer = 8;
            };

            // Destroy any existing shinies in the chest
            GameObject itemParent = chest.transform.Find("Item").gameObject;
            foreach (Transform t in itemParent.transform)
            {
                UnityEngine.Object.Destroy(t.gameObject);
            }

            return chest;
        }

        public static GameObject MakeNewChest(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GameObject chest = MakeNewChest(placement);

            var info = chest.GetOrAddComponent<ContainerInfo>();
            info.containerType = Container.Chest;
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
            FsmState activated = chestFsm.GetState("Activated");

            FsmStateAction checkAction = new Lambda(() => chestFsm.SendEvent(placement.CheckVisitedAny(VisitState.Opened) ? "ACTIVATE" : null));

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

            spawnItems.AddFirstAction(new Lambda(OnSpawnItems));
            spawnItems.AddFirstAction(new Lambda(() => placement.AddVisitFlag(VisitState.Opened)));
            activated.AddFirstAction(new Lambda(OnActivateChest));

            void OnSpawnItems()
            {
                GiveInfo info = new()
                {
                    Container = Container.Chest,
                    FlingType = flingType,
                    Transform = chestFsm.transform,
                    MessageType = MessageType.Corner,
                };

                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.Chest))
                        {
                            item.Give(placement, info);
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

            void OnActivateChest()
            {
                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                        ShinyUtility.PutShinyInContainer(itemParent, shiny);
                        ShinyUtility.FlingShinyRandomly(shiny.LocateFSM("Shiny Control"));
                    }
                }
            }
        }
    }
}
