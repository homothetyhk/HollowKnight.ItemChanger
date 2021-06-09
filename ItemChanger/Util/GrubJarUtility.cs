using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class GrubJarUtility
    {
        public const float GRUB_JAR_ELEVATION = 0.1f;

        public static GameObject MakeNewGrubJar(AbstractPlacement location)
        {
            GameObject grubJar = ObjectCache.GrubJar;
            grubJar.name = GetGrubJarName(location);

            return grubJar;
        }

        public static string GetGrubJarName(AbstractPlacement location)
        {
            return $"Grub Bottle-{location.name}";
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

        public static void ModifyBottleFsm(GameObject jar, FlingType flingType, AbstractPlacement location, IEnumerable<AbstractItem> items)
        {
            PlayMakerFSM fsm = jar.LocateFSM("Bottle Control");
            FsmState init = fsm.GetState("Init");
            FsmState shatter = fsm.GetState("Shatter");
            FsmState activate = fsm.GetState("Activate");

            init.RemoveActionsOfType<BoolTest>();
            shatter.RemoveActionsOfType<IncrementPlayerDataInt>();
            shatter.RemoveActionsOfType<SendMessage>();

            FsmStateAction checkAction = new Lambda(() => fsm.SendEvent(location.HasVisited() ? "ACTIVATE" : null));
            init.AddFirstAction(checkAction);

            GameObject itemParent = new GameObject("item");
            itemParent.transform.SetParent(jar.transform);
            itemParent.transform.position = jar.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            FsmStateAction spawnShinies = new ActivateAllChildren { gameObject = new FsmGameObject { Value = itemParent, }, activate = true };
            FsmStateAction removeParent = new Lambda(() => itemParent.transform.parent = null);
            shatter.AddAction(spawnShinies);
            activate.AddFirstAction(removeParent); // activate has a destroy all children action
            activate.AddFirstAction(spawnShinies);

            foreach (AbstractItem item in items)
            {
                if (item.GiveEarly(Container.GrubJar))
                {
                    FsmStateAction giveAction = new Lambda(() => item.Give(location, Container.GrubJar, flingType, jar.transform, MessageType.Corner));
                    shatter.AddAction(giveAction);
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
