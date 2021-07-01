using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ItemChanger
{
    public struct StartDef
    {
        public const string RESPAWN_MARKER_NAME = "ITEMCHANGER_RESPAWN_MARKER";
        public const string RESPAWN_TAG = "RespawnPoint";
        public string startSceneName;
        public float startX;
        public float startY;
        internal static StartDef start;

        internal static void OverrideStartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            if (permadeathMode) self.playerData.permadeathMode = 1;
            self.playerData.respawnScene = start.startSceneName;
            self.playerData.respawnMarkerName = RESPAWN_MARKER_NAME;
            self.StartCoroutine(self.RunContinueGame());
        }

        internal static void CreateRespawnMarker(Scene from, Scene to)
        {
            if (to.name == start.startSceneName)
            {
                GameObject marker = new GameObject();
                marker.name = StartDef.RESPAWN_MARKER_NAME;
                marker.tag = StartDef.RESPAWN_TAG;
                marker.transform.position = new Vector3(start.startX, start.startY, 7.4f);
            }
        }

        internal static Func<
            (string respawnScene, string respawnMarkerName, int respawnType, int mapZone),
            (string respawnScene, string respawnMarkerName, int respawnType, int mapZone)
            >
            BenchwarpGetStartDef = def => (start.startSceneName, RESPAWN_MARKER_NAME, 0, 2);

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
}
