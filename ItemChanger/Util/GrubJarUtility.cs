using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Components;
using ItemChanger.Extensions;
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

            // proper collsion layer to not collide with corpses while falling
            grubJar.transform.Find("Bottle Physical").gameObject.layer = 0;
            grubJar.layer = 0;


            var info = grubJar.AddComponent<ContainerInfo>();
            info.containerType = Container.GrubJar;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };


            grubJar.AddComponent<DropIntoPlace>().OnLand += () =>
            {
                grubJar.transform.Find("Bottle Physical").gameObject.layer = 8;
                grubJar.layer = 19;
            };

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
            grubJar.transform.position = new Vector3(pos.x, pos.y + GRUB_JAR_ELEVATION - elevation, -0.1f);

            var grub = grubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y, 0);
            grubJar.SetActive(target.activeSelf);
        }

        public static void MoveGrubJar(GameObject grubJar, float x, float y, float elevation)
        {
            grubJar.transform.position = new Vector3(x, y + GRUB_JAR_ELEVATION - elevation, -0.1f);
            var grub = grubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y + GRUB_JAR_ELEVATION - elevation, 0f);
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

            FsmStateAction checkAction = new DelegateBoolTest(
                () => placement.CheckVisitedAny(VisitState.Opened),
                "ACTIVATE", null);
            init.AddFirstAction(checkAction);

            GameObject itemParent = new GameObject("item");
            itemParent.transform.SetParent(jar.transform);
            itemParent.transform.position = jar.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            shatter.AddFirstAction(new DelegateBoolTest(() => CheckIfLanded(jar), null, "CANCEL"));
            activate.AddFirstAction(new Lambda(() =>
            {
                itemParent.transform.SetParent(null);
            }));

            activate.GetFirstActionOfType<DestroyAllChildren>().gameObject.Value = jar; // the target is null otherwise for some reason??

            FsmStateAction giveItems = new Lambda(InstantiateShiniesAndGiveEarly);
            shatter.AddLastAction(giveItems);
            activate.AddLastAction(giveItems);

            void InstantiateShiniesAndGiveEarly()
            {
                GiveInfo info = new GiveInfo
                {
                    Container = Container.GrubJar,
                    FlingType = flingType,
                    Transform = jar.transform,
                    MessageType = MessageType.Corner,
                };

                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.GrubJar))
                        {
                            item.Give(placement, info);
                        }
                        else
                        {
                            GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                            ShinyUtility.PutShinyInContainer(itemParent, shiny);
                        }
                    }
                }

                foreach (Transform t in itemParent.transform) t.gameObject.SetActive(true);
                placement.AddVisitFlag(VisitState.Opened);
            }
        }

        public static bool CheckIfLanded(GameObject jar)
        {
            DropIntoPlace dp = jar.GetComponent<DropIntoPlace>();
            return !dp || dp.Landed;
        }
    }
}
