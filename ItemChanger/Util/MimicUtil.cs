using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;


namespace ItemChanger.Util
{
    public static class MimicUtil
    {
        public static GameObject CreateNewMimic(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GameObject mimicParent = new($"Grub Mimic Parent-{placement.Name}");
            var box = mimicParent.AddComponent<BoxCollider2D>();
            box.size = new(2f, 2.1f);
            box.offset = new(0, -0.3f);
            mimicParent.layer = 19;
            mimicParent.SetActive(false);
            mimicParent.AddComponent<DropIntoPlace>();

            GameObject mimicBottle = ObjectCache.MimicBottle;
            mimicBottle.name = "Grub Mimic Bottle";
            GameObject mimicTop = ObjectCache.MimicTop;
            mimicTop.name = "Grub Mimic Top";

            mimicBottle.transform.SetParent(mimicParent.transform);
            mimicTop.transform.SetParent(mimicParent.transform);
            mimicBottle.SetActive(true);
            mimicTop.SetActive(true);

            mimicBottle.transform.localPosition = new(0, 0.3f, -0.1f);
            mimicTop.transform.localPosition = new(0, 0.25f, -0.1f);
            mimicTop.transform.Find("Grub Mimic 1").localPosition = new(0, 0, -0.1f);
            mimicTop.transform.Find("Grub Mimic 1").GetComponent<SetZ>().z = 0f;

            PlayMakerFSM bottleControl = mimicBottle.LocateFSM("Bottle Control");
            FsmState init = bottleControl.GetState("Init");
            init.Actions[0] = new BoolTestMod(() => placement.CheckVisitedAny(VisitState.Dropped), (BoolTest)init.Actions[0]);
            init.GetFirstActionOfType<SendEventByName>().eventTarget.gameObject.GameObject = mimicTop;
            FsmState shatter = bottleControl.GetState("Shatter");
            shatter.AddFirstAction(new Lambda(() => placement.AddVisitFlag(VisitState.Dropped)));
            shatter.GetActionsOfType<SendEventByName>()[1].eventTarget.gameObject.GameObject = mimicTop;

            ContainerInfo info = mimicTop.AddComponent<ContainerInfo>();
            info.containerType = Container.Mimic;
            info.giveInfo = new()
            {
                flingType = flingType,
                items = items,
                placement = placement,
            };

            return mimicParent;
        }

        public static void ModifyMimic(PlayMakerFSM mimicTopFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            GameObject mimic = mimicTopFsm.gameObject.FindChild("Grub Mimic 1");
            HealthManager hm = mimic.GetComponent<HealthManager>();

            FsmState init = mimicTopFsm.GetState("Init");
            init.Actions = new FsmStateAction[]
            {
                init.Actions[0],
                init.Actions[1],
                init.Actions[2],
                init.Actions[6],
                init.Actions[7],
                new BoolTestMod(() => placement.CheckVisitedAny(VisitState.Opened), (BoolTest)init.Actions[8])
                // the removed actions are all various tests to check if the mimic is dead
                // we tie it to the placement to make it easier to control
            };
            FsmState activate = mimicTopFsm.GetState("Activate");
            activate.AddFirstAction(new Lambda(GiveAll));
            hm.OnDeath += GiveAll;

            void GiveAll()
            {
                Vector2 pos = mimic.transform.position;

                GiveInfo info = new GiveInfo
                {
                    Container = Container.Mimic,
                    FlingType = flingType,
                    Transform = mimic.transform,
                    MessageType = MessageType.Corner,
                };

                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.Mimic)) item.Give(placement, info);
                        else
                        {
                            GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                            shiny.transform.position = pos;
                            shiny.SetActive(true);
                        }
                    }
                }
                placement.AddVisitFlag(VisitState.Opened);
            }
        }
    }
}
