using ItemChanger.Components;
using ItemChanger.Util;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Location which prevents DreamPlantOrbs from giving essence and gives items directly after completing the whispering root. Supports a HintBox around the whispering root.
    /// </summary>
    public class WhisperingRootLocation : AutoLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            if (SubscribedLocations.Count == 0)
            {
                HookRoots();
            }
            SubscribedLocations[UnsafeSceneName] = this;
        }

        protected override void OnUnload()
        {
            SubscribedLocations.Remove(UnsafeSceneName);
            if (SubscribedLocations.Count == 0)
            {
                UnhookRoots();
            }
        }

        public override GiveInfo GetGiveInfo()
        {
            var info = base.GetGiveInfo();
            info.MessageType = MessageType.Corner;
            return info;
        }

        private static readonly Dictionary<string, WhisperingRootLocation> SubscribedLocations = new();

        // Implementation follows.
        private static void HookRoots()
        {
            On.DreamPlant.Awake += OverrideDreamPlantAwake;
            On.DreamPlant.Start += AfterDreamPlantStart;
            On.DreamPlant.CheckOrbs += AfterDreamPlantCheckOrbs;
            On.DreamPlant.OnTriggerEnter2D += AfterDreamPlantOnTriggerEnter2D;
            IL.DreamPlantOrb.OnTriggerEnter2D += RemoveOrbEssence;
        }
        private static void UnhookRoots()
        {
            On.DreamPlant.Awake -= OverrideDreamPlantAwake;
            On.DreamPlant.Start -= AfterDreamPlantStart;
            On.DreamPlant.CheckOrbs -= AfterDreamPlantCheckOrbs;
            On.DreamPlant.OnTriggerEnter2D -= AfterDreamPlantOnTriggerEnter2D;
            IL.DreamPlantOrb.OnTriggerEnter2D -= RemoveOrbEssence;
        }

        private static void RemoveOrbEssence(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (cursor.TryGotoNext(
                i => i.MatchCall<GameManager>("get_instance"),
                i => i.MatchLdstr(nameof(PlayerData.dreamOrbs)),
                i => i.MatchCallvirt<GameManager>(nameof(GameManager.IncrementPlayerDataInt))))
            {
                cursor.RemoveRange(3);
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Action<DreamPlantOrb>>(orb =>
                {
                    if (!SubscribedLocations.ContainsKey(orb.gameObject.scene.name))
                    {
                        GameManager.instance.IncrementPlayerDataInt(nameof(PlayerData.dreamOrbs));
                    }
                });
            }
        }

        private static void AfterDreamPlantOnTriggerEnter2D(On.DreamPlant.orig_OnTriggerEnter2D orig, DreamPlant self, Collider2D collision)
        {
            orig(self, collision);

            if (SubscribedLocations.ContainsKey(self.gameObject.scene.name) && collision.tag == "Dream Attack" && self.GetCheckOrbRoutine() == null)
            {
                self.SetCheckOrbRoutine(self.StartCoroutine(self.CheckOrbs()));
            }
        }


        private static System.Collections.IEnumerator AfterDreamPlantCheckOrbs(On.DreamPlant.orig_CheckOrbs orig, DreamPlant self)
        {
            yield return orig(self);

            if (SubscribedLocations.TryGetValue(self.gameObject.scene.name, out WhisperingRootLocation loc))
            {
                loc.Placement.GiveAll(new GiveInfo 
                {
                    Transform = HeroController.instance.transform,
                    FlingType = loc.flingType,
                    Container = "WhisperingRoot",
                    MessageType = MessageType.Corner,
                });
            }
        }

        private static void AfterDreamPlantStart(On.DreamPlant.orig_Start orig, DreamPlant self)
        {
            orig(self);

            if (!SubscribedLocations.TryGetValue(self.gameObject.scene.name, out WhisperingRootLocation loc)) return;

            if (loc.Placement.AllObtained())
            {
                self.SetCompleted(true);
                self.SetActivated(true);
                var anim = self.GetComponent<tk2dSpriteAnimator>();
                if (anim)
                {
                    anim.Play("Completed");
                }
                if (self.dreamDialogue)
                {
                    self.dreamDialogue.SetActive(true);
                }
            }

            if (loc.GetItemHintActive() && self.dreamDialogue)
            {
                HintBox.Create(self.dreamDialogue.transform, loc.Placement); // the dream plant transform is too high up
            }
        }

        private static void OverrideDreamPlantAwake(On.DreamPlant.orig_Awake orig, DreamPlant self)
        {
            if (!SubscribedLocations.ContainsKey(self.gameObject.scene.name))
            {
                orig(self);
                return;
            }

            self.SetSpriteFlash(self.GetComponent<SpriteFlash>());
            self.SetAudioSource(self.GetComponent<AudioSource>());
            self.SetAnim(self.GetComponent<tk2dSpriteAnimator>());
            // removed persistent data events
        }
    }
}
