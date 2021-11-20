using ItemChanger.Internal;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction for setting the player's respawn point to the RespawnPoint found in the active scene.
    /// </summary>
    public class SetHardSaveAction : FsmStateAction
    {
        public override void OnEnter()
        {
            GameManager gm = Ref.GM;

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
                    "SetHardSaveAction present in scene with no respawn points: " +
                    Ref.GM.GetSceneNameString());
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
                    "SetHardSaveAction could not identify type of RespawnPoint object in scene " +
                    Ref.GM.GetSceneNameString());
                Finish();
                return;
            }

            gm.SaveGame();
            Finish();
        }
    }
}