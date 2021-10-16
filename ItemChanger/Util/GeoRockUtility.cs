using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;
using UnityEngine;
using System.Collections;
using ItemChanger.Components;
using ItemChanger.Internal;

namespace ItemChanger.Util
{
    public static class GeoRockUtility
    {
        public class GeoRockInfo : MonoBehaviour
        {
            public GeoRockSubtype type = GeoRockSubtype.Default;
        }

        public static Dictionary<GeoRockSubtype, float> Elevation = new Dictionary<GeoRockSubtype, float>()
        {
            [GeoRockSubtype.Default] = -0.8f,
            [GeoRockSubtype.Abyss] = -0.5f,
            [GeoRockSubtype.City] = 0,
            [GeoRockSubtype.Deepnest] = -0.6f,
            [GeoRockSubtype.Fung01] = -0.5f,
            [GeoRockSubtype.Fung02] = -0.5f,
            [GeoRockSubtype.Grave01] = 0.2f,
            [GeoRockSubtype.Grave02] = 0.2f,
            [GeoRockSubtype.GreenPath01] = -0.6f,
            [GeoRockSubtype.GreenPath02] = -0.7f,
            [GeoRockSubtype.Hive] = -0.2f,
            [GeoRockSubtype.Mine] = 0.1f,
            [GeoRockSubtype.Outskirts] = -0.8f,
            // Not the same as the elevation of the original rock because
            // we're shrinking it to half the size.
            [GeoRockSubtype.Outskirts420] = 0.3f
        };

        public static float GetElevation(GeoRockSubtype type)
        {
            if (Elevation.TryGetValue(type, out float value)) return value;
            return -0.8f;
        }

        public static GameObject MakeNewGeoRock(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            GeoRockSubtype type = items.OfType<GeoRockItem>().FirstOrDefault()?.geoRockSubtype ?? GeoRockSubtype.Default;
            GameObject rock = ObjectCache.GeoRock(type);
            rock.AddComponent<GeoRockInfo>().type = type;
            rock.name = GetGeoRockName(placement);

            rock.AddComponent<DropIntoPlace>();
            rock.GetComponent<BoxCollider2D>().isTrigger = false; // some rocks only have trigger colliders

            var info = rock.AddComponent<ContainerInfo>();
            info.containerType = Container.GeoRock;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };

            if (type == GeoRockSubtype.Outskirts420)
            {
                rock.transform.localScale *= 0.5f;
            }

            return rock;
        }

        public static void SetRockContext(GameObject rock, float x, float y, float elevation)
        {
            GeoRockSubtype rockType = rock.GetComponent<GeoRockInfo>()?.type ?? GeoRockSubtype.Default;
            rock.transform.position = new Vector3(x, y + GetElevation(rockType) - elevation, 0);
            rock.SetActive(true);
        }

        public static void SetRockContext(GameObject rock, GameObject target, float elevation)
        {
            if (target.transform.parent != null)
            {
                rock.transform.SetParent(target.transform.parent);
            }

            rock.transform.position = target.transform.position;
            rock.transform.localPosition = target.transform.localPosition;
            rock.transform.SetPositionZ(0);

            GeoRockSubtype rockType = rock.GetComponent<GeoRockInfo>()?.type ?? GeoRockSubtype.Default;
            rock.transform.position += Vector3.up * (GetElevation(rockType) - elevation);
            
            /*
            if (rockType == GeoRockSubtype.Outskirts420)
            {
                var t = rock.transform;
                t.localScale = new Vector3(t.localScale.x * 0.5f, t.localScale.y * 0.5f, t.localScale.z);
            }
            */

            rock.SetActive(target.activeSelf);
        }

        public static string GetGeoRockName(AbstractPlacement placement)
        {
            return $"Geo Rock-{placement.Name}";
        }

        public static void ModifyGeoRock(PlayMakerFSM rockFsm, FlingType flingType, AbstractPlacement placement, IEnumerable<AbstractItem> items)
        {
            GameObject rock = rockFsm.gameObject;

            FsmState init = rockFsm.GetState("Initiate");
            FsmState idle = rockFsm.GetState("Idle");
            FsmState hit = rockFsm.GetState("Hit");
            FsmState payout = rockFsm.GetState("Destroy");
            FsmState broken = rockFsm.GetState("Broken");

            FsmStateAction checkAction = new DelegateBoolTest(
                () => placement.CheckVisitedAny(VisitState.Opened),
                "BROKEN", null);

            init.RemoveActionsOfType<IntCompare>();
            init.AddLastAction(checkAction);

            idle.RemoveActionsOfType<SetPosition>(); // otherwise the rock warps back after falling

            hit.ClearTransitions();
            hit.Actions = new[] { new DelegateBoolTest(() => CheckRigidBodyStatus(rockFsm.gameObject), "HIT", "FINISHED") };
            hit.AddTransition("HIT", "Pause Frame");
            hit.AddTransition("FINISHED", "Idle");
            //hit.RemoveActionsOfType<FlingObjectsFromGlobalPool>();

            var payoutAction = payout.GetFirstActionOfType<FlingObjectsFromGlobalPool>();
            payoutAction.spawnMin.Value = 0;
            payoutAction.spawnMax.Value = 0;

            GameObject itemParent = new GameObject("item");
            itemParent.transform.SetParent(rock.transform);
            itemParent.transform.position = rock.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            FsmStateAction giveItems = new Lambda(InstantiateShiniesAndGiveEarly);
            payout.AddLastAction(giveItems);
            broken.AddLastAction(giveItems);

            void InstantiateShiniesAndGiveEarly()
            {
                GiveInfo info = new GiveInfo
                {
                    Container = Container.GrubJar,
                    FlingType = flingType,
                    Transform = rock.transform,
                    MessageType = MessageType.Corner,
                };

                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.GeoRock))
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

        public static bool CheckRigidBodyStatus(GameObject rock)
        {
            Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
            return !rb || (rb.constraints & RigidbodyConstraints2D.FreezePositionY) == RigidbodyConstraints2D.FreezePositionY;
        }
    }
}
