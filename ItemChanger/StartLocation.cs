using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ItemChanger
{
    public struct StartLocation
    {
        public const string RESPAWN_MARKER_NAME = "ITEMCHANGER_RESPAWN_MARKER";
        public const string RESPAWN_TAG = "RespawnPoint";
        public string startSceneName;
        public float startX;
        public float startY;
        internal static StartLocation start;

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
                marker.name = StartLocation.RESPAWN_MARKER_NAME;
                marker.tag = StartLocation.RESPAWN_TAG;
                marker.transform.position = new Vector3(start.startX, start.startY, 7.4f);
            }
        }

    }
}
