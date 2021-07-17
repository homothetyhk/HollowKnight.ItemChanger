using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Components;
using SereCore;
using UnityEngine;
using ItemChanger.Internal;

namespace ItemChanger.Util
{
    public static class GrubJarUtility
    {
        public const float GRUB_JAR_ELEVATION = 0.1f;

        public static GameObject MakeNewGrubJar(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GameObject grubJar = ObjectCache.GrubJar;
            grubJar.name = GetGrubJarName(placement);

            // proper collsion layer
            grubJar.transform.Find("Bottle Physical").gameObject.layer = 0;
            grubJar.layer = 0;

            var info = grubJar.AddComponent<ContainerInfo>();
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType
            };

            grubJar.AddComponent<DropIntoPlace>();

            return grubJar;
        }

        public static string GetGrubJarName(AbstractPlacement placement)
        {
            return $"Grub Bottle-{placement.Name}";
        }

        public static void MoveGrubJar(GameObject grubJar, GameObject target, float elevation)
        {
            if (target.transform.parent != null)
            {
                grubJar.transform.SetParent(target.transform.parent);
            }

            grubJar.transform.position = target.transform.position;
            grubJar.transform.localPosition = target.transform.localPosition;
            var pos = grubJar.transform.position;
            // Move the jar forward so it appears in front of any background objects
            grubJar.transform.position = new Vector3(pos.x, pos.y + GRUB_JAR_ELEVATION - elevation, pos.z - 0.1f);

            var grub = grubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y, pos.z);
            grubJar.SetActive(target.activeSelf);
        }

        public static void AdjustGrubJarPosition(GameObject grubJar, float elevation)
        {
            var pos = grubJar.transform.position;
            // Move the jar forward so it appears in front of any background objects
            grubJar.transform.position = new Vector3(pos.x, pos.y + GRUB_JAR_ELEVATION - elevation, pos.z - 0.1f);

            var grub = grubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y, pos.z);
        }

        public static void MoveGrubJar(GameObject grubJar, float x, float y, float elevation)
        {
            grubJar.transform.position = new Vector3(x, y, grubJar.transform.position.z - 0.1f);
            var grub = grubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y + GRUB_JAR_ELEVATION - elevation, grubJar.transform.position.z);
            grubJar.SetActive(true);
        }

        public static void ModifyBottleFsm(PlayMakerFSM bottleFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            GameObject jar = bottleFsm.gameObject;
            FsmState init = bottleFsm.GetState("Init");
            FsmState shatter = bottleFsm.GetState("Shatter");
            FsmState activate = bottleFsm.GetState("Activate");

            init.RemoveActionsOfType<BoolTest>();
            shatter.RemoveActionsOfType<IncrementPlayerDataInt>();
            shatter.RemoveActionsOfType<SendMessage>();

            FsmStateAction checkAction = new Lambda(() => bottleFsm.SendEvent(placement.CheckVisited() ? "ACTIVATE" : null));
            init.AddFirstAction(checkAction);

            GameObject itemParent = new GameObject("item");
            itemParent.transform.SetParent(jar.transform);
            itemParent.transform.position = jar.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            FsmStateAction spawnShinies = new ActivateAllChildren { gameObject = new FsmGameObject { Value = itemParent, }, activate = true };
            FsmStateAction removeParent = new Lambda(() => itemParent.transform.parent = null);

            shatter.AddFirstAction(new BoolTestMod(() => CheckRigidBodyStatus(jar), null, "CANCEL"));
            shatter.AddAction(spawnShinies);
            activate.AddFirstAction(removeParent); // activate has a destroy all children action
            activate.AddFirstAction(spawnShinies);

            foreach (AbstractItem item in items)
            {
                if (item.GiveEarly(Container.GrubJar))
                {
                    GiveInfo info = new GiveInfo
                    {
                        Container = Container.GrubJar,
                        FlingType = flingType,
                        Transform = jar.transform,
                        MessageType = MessageType.Corner,
                    };

                    FsmStateAction giveAction = new Lambda(() => item.Give(placement, info));
                    shatter.AddAction(giveAction);
                }
                else
                {
                    GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                    ShinyUtility.PutShinyInContainer(itemParent, shiny);
                }
            }
        }

        public static bool CheckRigidBodyStatus(GameObject jar)
        {
            Rigidbody2D rb = jar.GetComponent<Rigidbody2D>();
            return !rb || (rb.constraints & RigidbodyConstraints2D.FreezePositionY) == RigidbodyConstraints2D.FreezePositionY;
        }
    }
}
