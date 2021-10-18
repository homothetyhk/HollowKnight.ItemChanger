using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine;
using ItemChanger.Extensions;

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
        internal static StartDef Start => Internal.Ref.Settings.Start;

        public void ApplyToPlayerData(PlayerData pd)
        {
            pd.respawnMarkerName = RESPAWN_MARKER_NAME;
            pd.respawnScene = SceneName;
            pd.mapZone = (GlobalEnums.MapZone)MapZone;
        }

        public virtual void CreateRespawnMarker(Scene startScene)
        {
            GameObject marker = new()
            {
                name = StartDef.RESPAWN_MARKER_NAME,
                tag = StartDef.RESPAWN_TAG
            };
            marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
            marker.transform.position = new Vector3(Start.X, Start.Y, 7.4f);
        }

        internal static void Hook()
        {
            HookBenchwarp();
            Events.OnSceneChange += OnSceneChange;
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
    }

    public class RelativeStartDef : StartDef
    {
        /// <summary>
        /// The full path to the GameObject, with '/' separators.
        /// </summary>
        public string objPath;

        public override void CreateRespawnMarker(Scene startScene)
        {
            GameObject go = startScene.FindGameObject(objPath);
            GameObject marker = new()
            {
                name = StartDef.RESPAWN_MARKER_NAME,
                tag = StartDef.RESPAWN_TAG
            };
            marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
            marker.transform.position = new Vector3(go.transform.position.x + Start.X, go.transform.position.y + Start.Y, 7.4f);
        }
    }

    /// <summary>
    /// Accounts for the fact that transitions are not consistently placed as root gameobjects
    /// </summary>
    public class TransitionBasedStartDef : RelativeStartDef
    {
        [Newtonsoft.Json.JsonConstructor]
        private TransitionBasedStartDef() { }

        /// <summary>
        /// Attempts to find a start location near the corresponding transition. Unlikely to succeed for bottom transitions.
        /// </summary>
        public static TransitionBasedStartDef FromGate(string sceneName, string entryGateName, int mapZone = 2)
        {
            var def = new TransitionBasedStartDef
            {
                SceneName = sceneName,
                objPath = entryGateName,
                MapZone = mapZone,
                X = 0f,
                Y = 0f,
            };

            switch (entryGateName[0])
            {
                case 'l':
                    def.X += 1.5f;
                    break;
                case 'b':
                    def.X += 3f;
                    def.Y += 5f;
                    break;
                case 't':
                    def.Y -= 1.5f;
                    break;
                case 'r':
                    def.X -= 1.5f;
                    break;
                case 'd':
                    break;
            }

            return def;
        }

        public override void CreateRespawnMarker(Scene startScene)
        {
            GameObject[] objects = startScene.GetRootGameObjects();
            GameObject go = null;
            foreach (GameObject g in objects)
            {
                if (g.name == objPath && g.GetComponent<TransitionPoint>() != null)
                {
                    go = g;
                    break;
                }
                else if (g.name == "_Transition Gates" && g.GetComponentsInChildren<TransitionPoint>().FirstOrDefault(t => t.gameObject.name == objPath) is TransitionPoint tp)
                {
                    go = tp.gameObject;
                    break;
                }
            }
            if (go == null)
            {
                go = objects.SelectMany(g => g.GetComponentsInChildren<TransitionPoint>()).FirstOrDefault(t => t.gameObject.name == objPath).gameObject;
            }
            if (go == null) throw new ArgumentException($"Could not find transition point {objPath}.");

            GameObject marker = new()
            {
                name = StartDef.RESPAWN_MARKER_NAME,
                tag = StartDef.RESPAWN_TAG
            };
            marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
            marker.transform.position = new Vector3(go.transform.position.x + Start.X, go.transform.position.y + Start.Y, 7.4f);
        }
    }
}
