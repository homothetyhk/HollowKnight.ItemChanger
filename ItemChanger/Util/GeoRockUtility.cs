using ItemChanger.Items;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;
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

        public static GameObject MakeNewGeoRock(ContainerInfo info)
        {
            GeoRockSubtype type = GeoRockSubtype.Default;
            if (info.giveInfo.placement.GetPlacementAndLocationTags()
                .OfType<Tags.GeoRockSubtypeTag>()
                .FirstOrDefault() is Tags.GeoRockSubtypeTag grst)
            {
                type = grst.Type;
            }
            else
            {
                foreach (AbstractItem i in info.giveInfo.items)
                {
                    if (i is GeoRockItem gr)
                    {
                        type = gr.geoRockSubtype;
                        break;
                    }
                }
            }
            
            GameObject rock = ObjectCache.GeoRock(ref type);
            rock.AddComponent<GeoRockInfo>().type = type;
            rock.name = GetGeoRockName(info.giveInfo.placement);

            rock.GetComponent<BoxCollider2D>().isTrigger = false; // some rocks only have trigger colliders
            rock.AddComponent<DropIntoPlace>();

            rock.AddComponent<ContainerInfoComponent>().info = info;

            if (type == GeoRockSubtype.Outskirts420)
            {
                rock.transform.localScale *= 0.5f;
            }

            return rock;
        }

        public static GameObject MakeNewGeoRock(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType)
        {
            return MakeNewGeoRock(new ContainerInfo(Container.GeoRock, placement, items, flingType));
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


            init.RemoveActionsOfType<IntCompare>();
            init.AddLastAction(new DelegateBoolTest(
                () => placement.CheckVisitedAny(VisitState.Opened),
                "BROKEN", null));

            idle.RemoveActionsOfType<SetPosition>(); // otherwise the rock warps back after falling

            hit.ClearTransitions();
            hit.SetActions(new DelegateBoolTest(() => CheckIfLanded(rockFsm.gameObject), "HIT", "FINISHED"));
            hit.AddTransition("HIT", "Pause Frame");
            hit.AddTransition("FINISHED", "Idle");

            var payoutAction = payout.GetFirstActionOfType<FlingObjectsFromGlobalPool>();
            payoutAction.spawnMin.Value = 0;
            payoutAction.spawnMax.Value = 0;

            GameObject itemParent = new("item");
            itemParent.transform.SetParent(rock.transform);
            itemParent.transform.position = rock.transform.position;
            itemParent.transform.localPosition = Vector3.zero;
            itemParent.SetActive(true);

            payout.AddLastAction(new Lambda(InstantiateShiniesAndGiveEarly));
            broken.RemoveFirstActionOfType<SetCollider>();
            broken.AddLastAction(new Lambda(OnAlreadyBroken));

            void InstantiateShiniesAndGiveEarly()
            {
                GiveInfo info = new()
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
                            if (flingType == FlingType.Everywhere) ShinyUtility.FlingShinyRandomly(shiny.LocateMyFSM("Shiny Control"));
                            else ShinyUtility.FlingShinyDown(shiny.LocateMyFSM("Shiny Control"));
                        }
                    }
                }

                foreach (Transform t in itemParent.transform) t.gameObject.SetActive(true);
                placement.AddVisitFlag(VisitState.Opened);
            }

            void OnAlreadyBroken()
            {
                foreach (AbstractItem item in items)
                {
                    if (!item.IsObtained())
                    {
                        GameObject shiny = ShinyUtility.MakeNewShiny(placement, item, flingType);
                        ShinyUtility.PutShinyInContainer(itemParent, shiny);
                        if (flingType == FlingType.Everywhere) ShinyUtility.FlingShinyRandomly(shiny.LocateMyFSM("Shiny Control"));
                        else ShinyUtility.FlingShinyDown(shiny.LocateMyFSM("Shiny Control"));
                    }
                }

                rock.GetOrAddComponent<NonBouncer>();
                foreach (Transform t in itemParent.transform) t.gameObject.SetActive(true);
            }
        }

        public static bool CheckIfLanded(GameObject rock)
        {
            DropIntoPlace dp = rock.GetComponent<DropIntoPlace>();
            return !dp || dp.Landed;
        }
    }
}
