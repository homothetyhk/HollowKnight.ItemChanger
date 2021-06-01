using HutongGames.PlayMaker;
using SereCore;
using UnityEngine;
using static Modding.Logger;

namespace ItemChanger.FsmStateActions
{
    internal class RandomizerSetHardSave : FsmStateAction
    {
        public override void OnEnter()
        {
            GameManager gm = SereCore.Ref.GM;

            if (gm == null)
            {
                Finish();
                return;
            }

            PlayerData pd = gm.playerData;

            if (pd == null)
            {
                Finish();
                return;
            }

            GameObject spawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint");

            if (spawnPoint == null)
            {
                LogWarn(
                    "RandomizerSetHardSave action present in scene with no respawn points: " +
                    SereCore.Ref.GM.GetSceneNameString());
                Finish();
                return;
            }

            PlayMakerFSM bench = FSMUtility.LocateFSM(spawnPoint, "Bench Control");
            RespawnMarker marker = spawnPoint.GetComponent<RespawnMarker>();
            if (bench != null)
            {
                pd.SetBenchRespawn(spawnPoint.name, gm.GetSceneNameString(), 1, true);
            }
            else if (marker != null)
            {
                pd.SetBenchRespawn(marker, gm.GetSceneNameString(), 2);
            }
            else
            {
                LogWarn(
                    "RandomizerSetHardSave could not identify type of RespawnPoint object in scene " +
                    SereCore.Ref.GM.GetSceneNameString());
                Finish();
                return;
            }

            gm.SaveGame();
            Finish();
        }
    }
}