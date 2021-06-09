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
using System.Collections;

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

        public static GameObject MakeNewGeoRock(AbstractPlacement location, IEnumerable<AbstractItem> items, out GeoRockSubtype type)
        {
            type = items.OfType<GeoRockItem>().FirstOrDefault()?.geoRockSubtype ?? GeoRockSubtype.Default;
            GameObject rock = ObjectCache.GeoRock(type);
            rock.AddComponent<GeoRockInfo>().type = type;
            rock.name = GetGeoRockName(location);
            return rock;
        }

        public static void SetRockContext(GameObject rock, float x, float y, float elevation)
        {
            GeoRockSubtype rockType = rock.GetComponent<GeoRockInfo>()?.type ?? GeoRockSubtype.Default;
            rock.transform.position = new Vector3(x, y + GetElevation(rockType) - elevation);

            if (rockType == GeoRockSubtype.Outskirts420)
            {
                var t = rock.transform;
                t.localScale = new Vector3(t.localScale.x * 0.5f, t.localScale.y * 0.5f, t.localScale.z);
            }
            rock.SetActive(true);
        }

        public static void SetRockContext(GameObject rock, GameObject target, float elevation)
        {
            bool isSpecialLocation = target.transform.parent != null &&
                    !(target.transform.parent.position.x == 0f && target.transform.parent.position.y == 0f);

            if (target.transform.parent != null && !isSpecialLocation)
            {
                rock.transform.SetParent(target.transform.parent);
            }

            rock.transform.position = target.transform.position;
            if (!isSpecialLocation)
            {
                rock.transform.localPosition = target.transform.localPosition;
            }

            GeoRockSubtype rockType = rock.GetComponent<GeoRockInfo>()?.type ?? GeoRockSubtype.Default;
            rock.transform.position += Vector3.up * (GetElevation(rockType) - elevation);
            if (rockType == GeoRockSubtype.Outskirts420)
            {
                var t = rock.transform;
                t.localScale = new Vector3(t.localScale.x * 0.5f, t.localScale.y * 0.5f, t.localScale.z);
            }
            rock.SetActive(target.activeSelf);
        }

        public static string GetGeoRockName(AbstractPlacement location)
        {
            return $"Geo Rock-{location.name}";
        }

        public static void ModifyGeoRock(PlayMakerFSM rockFsm, FlingType flingType, AbstractPlacement location, IEnumerable<AbstractItem> items)
        {
            GameObject rock = rockFsm.gameObject;

            FsmState init = rockFsm.GetState("Initiate");
            FsmState hit = rockFsm.GetState("Hit");
            FsmState payout = rockFsm.GetState("Destroy");
            FsmState broken = rockFsm.GetState("Broken");

            FsmStateAction checkAction = new Lambda(() => rockFsm.SendEvent(location.HasVisited() ? "BROKEN" : null));

            init.RemoveActionsOfType<IntCompare>();
            init.AddAction(checkAction);

            hit.ClearTransitions();
            hit.AddTransition("HIT", "Pause Frame");
            hit.AddTransition("FINISHED", "Pause Frame");
            hit.RemoveActionsOfType<FlingObjectsFromGlobalPool>();

            var payoutAction = payout.GetActionOfType<FlingObjectsFromGlobalPool>();
            payoutAction.spawnMin.Value = 0;
            payoutAction.spawnMax.Value = 0;

            GameObject itemParent = new GameObject("item");
            itemParent.transform.SetParent(rock.transform);
            itemParent.transform.position = rock.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            FsmStateAction spawnShinies = new ActivateAllChildren { gameObject = new FsmGameObject { Value = itemParent, }, activate = true };
            payout.AddAction(spawnShinies);
            broken.AddAction(spawnShinies);

            foreach (AbstractItem item in items)
            {
                if (item.GiveEarly(Container.GeoRock))
                {
                    FsmStateAction giveAction = new Lambda(() => item.Give(location, Container.GeoRock, flingType, rockFsm.gameObject.transform, message: MessageType.None));
                    payout.AddAction(giveAction);
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
