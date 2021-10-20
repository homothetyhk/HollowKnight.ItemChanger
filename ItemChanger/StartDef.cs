using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace ItemChanger
{

    public class StartDef
    {
        public const string RESPAWN_MARKER_NAME = "ITEMCHANGER_RESPAWN_MARKER";
        public const string RESPAWN_TAG = "RespawnPoint";
        public virtual string SceneName { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public virtual int MapZone { get; set; } = 2;
        public virtual bool RespawnFacingRight { get; set; } = true;
        public virtual SpecialStartEffects SpecialEffects { get; set; } = SpecialStartEffects.Default;
        internal static StartDef Start => Internal.Ref.Settings.Start;

        public class StartComponent : MonoBehaviour
        {
            public StartDef def;
        }

        public void ApplyToPlayerData(PlayerData pd)
        {
            pd.respawnMarkerName = RESPAWN_MARKER_NAME;
            pd.respawnScene = SceneName;
            pd.mapZone = (GlobalEnums.MapZone)MapZone;
        }

        public GameObject CreateRespawnMarker(Vector3 pos)
        {
            GameObject marker = new()
            {
                name = StartDef.RESPAWN_MARKER_NAME,
                tag = StartDef.RESPAWN_TAG
            };
            marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
            marker.AddComponent<StartComponent>().def = this;
            marker.transform.position = pos;
            return marker;
        }

        public virtual void CreateRespawnMarker(Scene startScene)
        {
            GameObject marker = new()
            {
                name = StartDef.RESPAWN_MARKER_NAME,
                tag = StartDef.RESPAWN_TAG
            };
            marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
            marker.AddComponent<StartComponent>().def = this;
            marker.transform.position = new Vector3(Start.X, Start.Y, 7.4f);
        }

        internal static void Hook()
        {
            HookBenchwarp();
            Events.OnSceneChange += OnSceneChange;
            On.HeroController.Respawn += OnRespawn;
        }

        internal static void Unhook()
        {
            UnHookBenchwarp();
            Events.OnSceneChange -= OnSceneChange;
        }

        internal static void OnSceneChange(Scene to)
        {
            if (Start != null && to.name == Start.SceneName)
            {
                Start.CreateRespawnMarker(to);
            }
        }

        internal static Func<
            (string respawnScene, string respawnMarkerName, int respawnType, int mapZone),
            (string respawnScene, string respawnMarkerName, int respawnType, int mapZone)
            >
            BenchwarpGetStartDef = def => Start == null ? def : (Start.SceneName, RESPAWN_MARKER_NAME, 0, Start.MapZone);

        internal static void HookBenchwarp()
        {
            try
            {
                FieldInfo field = Type.GetType("Benchwarp.Events, Benchwarp")
                    .GetField("OnGetStartDef", BindingFlags.Public | BindingFlags.Static);

                field.FieldType
                    .GetEvent("Event", BindingFlags.Public | BindingFlags.Instance)
                    .AddEventHandler(field.GetValue(null), BenchwarpGetStartDef);
            }
            catch
            {
                return;
            }
        }

        internal static void UnHookBenchwarp()
        {
            try
            {
                FieldInfo field = Type.GetType("Benchwarp.Events, Benchwarp")
                    .GetField("OnGetStartDef", BindingFlags.Public | BindingFlags.Static);

                field.FieldType
                    .GetEvent("Event", BindingFlags.Public | BindingFlags.Instance)
                    .RemoveEventHandler(field.GetValue(null), BenchwarpGetStartDef);
            }
            catch
            {
                return;
            }
        }

        private static IEnumerator OnRespawn(On.HeroController.orig_Respawn orig, HeroController self)
        {
            IEnumerator e = orig(self);
            FieldInfo state = e.GetType().GetField("<>1__state", BindingFlags.Instance | BindingFlags.NonPublic);
            int GetState() => (int)state.GetValue(e);
            void SetState(int i) => state.SetValue(e, i);
            FieldInfo spawn = e.GetType().GetField("<spawnPoint>5__2", BindingFlags.Instance | BindingFlags.NonPublic);
            Transform GetSpawnPoint() => (Transform)spawn.GetValue(e);

            while (e.MoveNext())
            {
                yield return e.Current;
                if (GetState() == 2 && GameManager.instance.GetSceneNameString() != "GG_Atrium")
                {
                    // HC.Respawn
                    self.IgnoreInput();
                    RespawnMarker rm = GetSpawnPoint().GetComponent<RespawnMarker>();
                    if (rm && !rm.respawnFacingRight) self.FaceLeft();
                    else self.FaceRight();
                    (typeof(HeroController).GetField("heroInPosition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) as HeroController.HeroInPosition)?.Invoke(false);
                    var hac = self.GetComponent<HeroAnimationController>();
                    float wakeUpTime = hac.GetClipDuration("Wake Up Ground"); // 46 frames, 18 fps, 2.55s
                    // break from HC.Respawn
                    var sc = rm.GetComponent<StartComponent>();
                    if (sc && sc.def != null && (sc.def.SpecialEffects & SpecialStartEffects.DelayedWake) != 0)
                    {
                        self.playerData.isInvincible = true;
                        hac.PlayClip("Prostrate");
                        self.StopAnimationControl();
                        self.controlReqlinquished = true;
                        yield return DoDelayedRespawn(sc.def.SpecialEffects);
                        hac.animator.PlayFrom("Wake Up Ground", 1.56f);
                        yield return new WaitForSeconds(wakeUpTime - 1.56f);
                    }
                    else
                    {
                        hac.PlayClip("Wake Up Ground");
                        self.StopAnimationControl();
                        self.controlReqlinquished = true;
                        yield return new WaitForSeconds(wakeUpTime);
                    }

                    self.StartAnimationControl();
                    self.controlReqlinquished = false;
                    self.proxyFSM.SendEvent("HeroCtrl-Respawned");
                    typeof(HeroController).GetMethod("FinishedEnteringScene", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { true, false });
                    self.playerData.disablePause = false;
                    if (sc && sc.def != null && (sc.def.SpecialEffects & SpecialStartEffects.ExtraInvincibility) != 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                    }
                    self.playerData.isInvincible = false;
                    SetState(-1);
                    break;
                }
            }
        }

        private static IEnumerator DoDelayedRespawn(SpecialStartEffects effects)
        {
            float t = 0f;
            float u = 0f;
            bool refillSoul = (effects & SpecialStartEffects.SlowSoulRefill) == SpecialStartEffects.SlowSoulRefill;
            HeroActions ia = InputHandler.Instance.inputActions;

            while (true)
            {
                if (ia.jump.IsPressed || ia.attack.IsPressed || ia.cast.IsPressed || ia.left.IsPressed || ia.right.IsPressed || ia.up.IsPressed || ia.down.IsPressed)
                {
                    yield break;
                }
                else if (t < 4f)
                {
                    t += 0.02f;
                }
                else if (refillSoul && u > 0.09f)
                {
                    HeroController.instance.TryAddMPChargeSpa(1);
                    u = 0f;
                }
                else
                {
                    u += 0.02f;
                }
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}
