using System.Collections;
using ItemChanger.Internal;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction with static methods for flinging soul from a transform.
    /// </summary>
    public class FlingSoulAction : FsmStateAction
    {
        private readonly GameObject _gameObject;

        public FlingSoulAction(GameObject baseObj)
        {
            _gameObject = baseObj;
        }

        public override void OnEnter()
        {
            SpawnSoul(_gameObject.transform, 100, 11);

            Finish();
        }

        public static void SpawnSoul(Transform transform, int total)
        {
            GameObject soulPrefab = ObjectCache.SoulOrb;
            // Workaround because Spawn extension is slightly broken
            UObject.Destroy(soulPrefab.Spawn());
            soulPrefab.SetActive(true);

            FlingUtils.Config flingConfig = new()
            {
                Prefab = soulPrefab,
                AmountMin = total,
                AmountMax = total,
                SpeedMin = 10f,
                SpeedMax = 20f,
                AngleMin = 0f,
                AngleMax = 360f
            };

            FlingUtils.SpawnAndFling(flingConfig, transform, new Vector3(0f, 0f, 0f));
            soulPrefab.SetActive(false);
        }

        public static void SpawnSoul(Transform transform, int total, int spawnsPerFrame)
        {
            HeroController.instance.StartCoroutine(SpawnSoulRoutine(transform.position, total, spawnsPerFrame));
        }


        private static IEnumerator SpawnSoulRoutine(Vector3 position, int total, int spawnsPerFrame)
        {
            Transform spawnPoint = new GameObject("Soul Spawn Point").transform;
            spawnPoint.position = position;

            GameObject soulPrefab = ObjectCache.SoulOrb;
            // Workaround because Spawn extension is slightly broken
            UObject.Destroy(soulPrefab.Spawn());
            soulPrefab.SetActive(true);

            FlingUtils.Config flingConfig = new()
            {
                Prefab = soulPrefab,
                AmountMin = spawnsPerFrame,
                AmountMax = spawnsPerFrame,
                SpeedMin = 10f,
                SpeedMax = 20f,
                AngleMin = 0f,
                AngleMax = 360f
            };

            total = (total / 2) + (total % 2); // soul orbs give 2 soul each

            for (int i = 0; i < total / spawnsPerFrame; i++)
            {
                FlingUtils.SpawnAndFling(flingConfig, spawnPoint, new Vector3(0f, 0f, 0f));
                yield return null;
            }

            flingConfig.AmountMin = flingConfig.AmountMax = total % spawnsPerFrame;
            FlingUtils.SpawnAndFling(flingConfig, spawnPoint, new Vector3(0f, 0f, 0f));
            soulPrefab.SetActive(false);
        }
    }
}