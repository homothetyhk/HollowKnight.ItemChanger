using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Items;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    public class SoulTotemContainer : Container
    {
        public static readonly Color WasEverObtainedColor = new(1f, 213f / 255f, 0.5f);

        public class SoulTotemInfo : MonoBehaviour
        {
            public SoulTotemSubtype type;
        }

        public override string Name => Container.Totem;
        public override bool SupportsInstantiate => ObjectCache.SoulTotemPreloader.PreloadLevel != PreloadLevel.None;
        public override bool SupportsDrop => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null, Transition? changeSceneTo = null)
        {
            SoulTotemSubtype type = items.OfType<SoulTotemItem>().FirstOrDefault()?.soulTotemSubtype ?? SoulTotemSubtype.B;
            GameObject totem = ObjectCache.SoulTotem(ref type);
            totem.AddComponent<SoulTotemInfo>().type = type;
            totem.name = GetNewSoulTotemName(placement);
            
            if (ShrinkageFactor.TryGetValue(type, out var k))
            {
                var t = totem.transform;
                t.localScale = new Vector3(t.localScale.x * k, t.localScale.y * k, t.localScale.z);
            }

            totem.AddComponent<DropIntoPlace>();
            totem.GetComponent<BoxCollider2D>().isTrigger = false; // some rocks only have trigger colliders


            ContainerInfo info = totem.AddComponent<ContainerInfo>();
            info.containerType = Container.Totem;
            info.giveInfo = new()
            {
                items = items,
                flingType = flingType,
                placement = placement,
            };

            return totem;
        }

        /// <remarks>
        /// Soul totems are implemented as follows:
        /// - The totem type is the type of the first SoulTotemItem in the items list, or B if no SoulTotemItems were included. It is also B if Soul Totem preloads were reduced.
        /// - The number of hits of the totem is the sum of the number of hits of each of the SoulTotemItems. If any of the SoulTotemItems have a negative number of hits, the totem is infinite.
        /// - The totem spawns non-soul items as shinies on the first hit. If the totem is depleted but has items, it will still spawn items on the first hit, but not give soul.
        /// - Soul items also count as given after the first hit. If all of the items on the soul totem have been obtained at least once, the totem becomes tinted orange.
        /// - The number of hits left on the totem is saved by its PersistentIntItem component.
        /// </remarks>
        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            FsmState init = fsm.GetState("Init");
            FsmState hit = fsm.GetState("Hit");
            FsmState far = fsm.GetState("Far");
            FsmState close = fsm.GetState("Close");
            FsmState checkIfNail = fsm.GetState("Check if Nail");

            FsmBool activated = fsm.FsmVariables.FindFsmBool("Activated");
            FsmInt value = fsm.FsmVariables.FindFsmInt("Value");
            
            if (init.Transitions.Length < 2) // modify PoP fsm to match usual fsm
            {
                FsmGameObject emitter = fsm.FsmVariables.FindFsmGameObject("Emitter");
                FsmOwnerDefault emitterOwnerDefault = new() { GameObject = emitter, OwnerOption = OwnerDefaultOption.SpecifyGameObject, };
                FsmGameObject glower = fsm.FsmVariables.FindFsmGameObject("Glower");
                FsmOwnerDefault self = new();
                FsmGameObject hero = fsm.FsmVariables.FindFsmGameObject("Hero");
                FsmFloat distance = fsm.FsmVariables.FindFsmFloat("Distance");

                FsmState depleted = new(fsm.Fsm)
                {
                    Name = "Depleted",
                    Transitions = Array.Empty<FsmTransition>(),
                    Actions = new FsmStateAction[]
                    {
                        new SetCollider{ gameObject = self, active = false, },
                        new DestroyObject{ gameObject = emitter, delay = 0, detachChildren = false, },
                        new DestroyObject{ gameObject = glower, delay = 0, detachChildren = false, },
                        new ActivateGameObject{ gameObject = emitterOwnerDefault, activate = false, recursive = false, resetOnExit = false, everyFrame = false },
                        new SetBoolValue{ boolVariable = activated, boolValue = true, everyFrame = false },
                        new SetParticleEmission{ gameObject = emitterOwnerDefault, emission = false },
                        new GetDistance{ gameObject = self, target = hero,  },
                        far.GetFirstActionOfType<GetMaterialColor>(),
                        far.GetFirstActionOfType<EaseColor>(),
                        far.GetFirstActionOfType<SetMaterialColor>(),
                    },
                };
                fsm.AddState(depleted);

                FsmState meshRendererOff = new(fsm.Fsm)
                {
                    Name = "Mesh Renderer Off",
                    Transitions = new[] { new FsmTransition { FsmEvent = FsmEvent.Finished, ToFsmState = depleted, ToState = depleted.Name } },
                    Actions = new FsmStateAction[]
                    {
                        new SetSpriteRenderer{ gameObject = self, active = false },
                    },
                };
                fsm.AddState(meshRendererOff);

                init.AddTransition(FsmEvent.GetFsmEvent("DEPLETED"), meshRendererOff);
                hit.AddTransition(FsmEvent.GetFsmEvent("DEPLETED"), depleted);
            }

            PersistentIntItem pii = fsm.GetComponent<PersistentIntItem>();
            if (pii == null)
            {
                void OnAwake(On.PersistentIntItem.orig_Awake orig, PersistentIntItem self)
                {
                    if (self.persistentIntData == null) self.persistentIntData = new() { id = self.gameObject.name, sceneName = self.gameObject.scene.name, semiPersistent = self.semiPersistent };
                    orig(self);
                }
                On.PersistentIntItem.Awake += OnAwake;
                pii = fsm.gameObject.AddComponent<PersistentIntItem>();
                On.PersistentIntItem.Awake -= OnAwake;
            }
            PersistentIntData pid = pii.persistentIntData ??= new();
            pid.id = fsm.gameObject.name;
            pid.sceneName = fsm.gameObject.scene.name;

            FsmBool spawnedItems = fsm.AddFsmBool("Spawned Items", false);
            FsmState giveItems = new(fsm.Fsm)
            {
                Name = "Give Items",
                Transitions = new[] { new FsmTransition { FsmEvent = FsmEvent.Finished, ToFsmState = hit, ToState = hit.Name, } },
                Actions = new FsmStateAction[]
                {
                    new Lambda(InstantiateShiniesAndGiveEarly),
                },
            };
            fsm.AddState(giveItems);
            checkIfNail.Transitions.First(t => t.EventName == "DAMAGED").SetToState(giveItems);


            bool shouldBeInfinite = info.items.OfType<SoulTotemItem>().Any(i => i.hitCount < 0);
            fsm.FsmVariables.GetFsmInt("Value").Value = 0;
            fsm.GetState("Reset?").GetFirstActionOfType<SetIntValue>().intValue.Value = 0;
            fsm.GetState("Reset").GetFirstActionOfType<SetIntValue>().intValue.Value = 0;

            if (shouldBeInfinite)
            {
                // PoP totems, or fake PoP totems, should be infinite
                init.RemoveTransitionsTo("Mesh Renderer Off");
                hit.RemoveTransitionsTo("Depleted");
            }

            init.RemoveActionsOfType<BoolTest>();
            init.RemoveActionsOfType<IntCompare>();
            init.AddLastAction(new DelegateBoolTest(DepletedTest, FsmEvent.GetFsmEvent("DEPLETED"), FsmEvent.Finished));

            if (info.items.All(i => i.WasEverObtained()))
            {
                fsm.GetState("Close").GetFirstActionOfType<EaseColor>().toValue = WasEverObtainedColor;
            }

            {
                // these actions are not in a consistent order for different totem fsms
                var aposs = hit.GetFirstActionOfType<AudioPlayerOneShotSingle>();
                var smc = hit.GetFirstActionOfType<SetMaterialColor>();
                var sofgp0 = hit.Actions.OfType<SpawnObjectFromGlobalPool>().First();
                var sofgp1 = hit.Actions.OfType<SpawnObjectFromGlobalPool>().ElementAt(1);
                var sebn = hit.GetFirstActionOfType<SendEventByName>();
                var fofgp = hit.GetFirstActionOfType<FlingObjectsFromGlobalPool>();
                var sp = hit.GetFirstActionOfType<SetProperty>();
                var io = hit.GetFirstActionOfType<IntOperator>();
                var ic = hit.GetFirstActionOfType<IntCompare>();
                var w = hit.GetFirstActionOfType<Wait>();

                hit.Actions = new FsmStateAction[]
                {
                    aposs, smc, sofgp0, sofgp1, sebn,
                    new DelegateBoolTest(() => value.Value <= 0, FsmEvent.GetFsmEvent("DEPLETED"), null),
                    fofgp, sp, io, ic, w
                };
            }

            bool DepletedTest()
            {
                if (info.items.Any(i => !i.IsObtained())) return false;
                if (activated.Value) return true;
                return value.Value <= 0;
            }

            // prevent the totem from falling out of the scene when it's depleted before landing
            if (fsm.GetComponent<DropIntoPlace>())
            {
                FsmState depleted = fsm.GetState("Depleted");
                depleted.RemoveFirstActionOfType<SetCollider>();
                depleted.AddFirstAction(new Lambda(() =>
                {
                    void DisableCollider() => fsm.GetComponent<BoxCollider2D>().enabled = false;

                    var d = fsm.GetComponent<DropIntoPlace>();
                    if (!d || d.Landed) DisableCollider();
                    else d.OnLand += DisableCollider;
                }));
            }
            
            

            void InstantiateShiniesAndGiveEarly()
            {
                void TotemCallback(AbstractItem item)
                {
                    if (item is SoulTotemItem totemItem) value.Value += totemItem.hitCount;
                }

                GiveInfo gi = new()
                {
                    Container = Container.Totem,
                    FlingType = info.flingType,
                    Transform = fsm.transform,
                    MessageType = MessageType.Corner,
                    Callback = TotemCallback,
                };
                GameObject itemParent = new("Item parent");
                itemParent.transform.position = fsm.transform.position;

                foreach (AbstractItem item in info.items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.Totem))
                        {
                            item.Give(info.placement, gi.Clone());
                        }
                        else if (!spawnedItems.Value)
                        {
                            GameObject shiny = ShinyUtility.MakeNewShiny(info.placement, item, info.flingType);
                            ShinyUtility.PutShinyInContainer(itemParent, shiny);
                            if (info.flingType == FlingType.Everywhere) ShinyUtility.FlingShinyRandomly(shiny.LocateMyFSM("Shiny Control"));
                            else ShinyUtility.FlingShinyDown(shiny.LocateMyFSM("Shiny Control"));
                        }
                    }
                }

                foreach (Transform t in itemParent.transform)
                {
                    t.gameObject.SetActive(true);
                }
                info.placement.AddVisitFlag(VisitState.Opened);
                spawnedItems.Value = true;
            }
        }

        public override void ApplyTargetContext(GameObject obj, float x, float y, float elevation)
        {
            base.ApplyTargetContext(obj, x, y, elevation);
            obj.transform.Translate(new(0, Elevation[GetSoulTotemSubtype(obj)], 0));
        }

        public override void ApplyTargetContext(GameObject obj, GameObject target, float elevation)
        {
            base.ApplyTargetContext(obj, target, elevation);
            obj.transform.Translate(new(0, Elevation[GetSoulTotemSubtype(obj)], 0));
        }

        public static string GetNewSoulTotemName(AbstractPlacement placement)
        {
            return $"Soul Totem-{placement.Name}";
        }

        public static SoulTotemSubtype GetSoulTotemSubtype(GameObject totem)
        {
            if (totem.GetComponent<SoulTotemInfo>() is SoulTotemInfo totemInfo && totemInfo != null)
            {
                return totemInfo.type;
            }
            else return SoulTotemSubtype.B;
        }

        public static readonly Dictionary<SoulTotemSubtype, int> HitCount = new()
        {
            [SoulTotemSubtype.A] = 5,
            [SoulTotemSubtype.B] = 3,
            [SoulTotemSubtype.C] = 3,
            [SoulTotemSubtype.D] = 5,
            [SoulTotemSubtype.E] = 5,
            [SoulTotemSubtype.F] = 5,
            [SoulTotemSubtype.G] = 5,
            [SoulTotemSubtype.Palace] = 5,
            [SoulTotemSubtype.PathOfPain] = -1,
        };


        public static readonly Dictionary<SoulTotemSubtype, float> ShrinkageFactor = new()
        {
            [SoulTotemSubtype.D] = 0.7f,
            [SoulTotemSubtype.E] = 0.7f,
            [SoulTotemSubtype.Palace] = 0.8f,
            [SoulTotemSubtype.PathOfPain] = 0.7f,
        };

        public static readonly Dictionary<SoulTotemSubtype, float> Elevation = new()
        {
            [SoulTotemSubtype.A] = 0.5f,
            [SoulTotemSubtype.B] = -0.1f,
            [SoulTotemSubtype.C] = -0.1f,
            // Some elevation values adjusted from the originals to account for the shrinkage.
            [SoulTotemSubtype.D] = 1.3f - 0.5f,
            [SoulTotemSubtype.E] = 1.2f - 0.5f,
            [SoulTotemSubtype.F] = 0.8f,
            [SoulTotemSubtype.G] = 0.2f,
            [SoulTotemSubtype.Palace] = 1.3f - 0.3f,
            [SoulTotemSubtype.PathOfPain] = 1.5f - 0.9f,
        };
    }
}
