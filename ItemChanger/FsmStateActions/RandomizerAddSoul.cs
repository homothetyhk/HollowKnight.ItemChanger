using System.Collections;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.FsmStateActions
{
    public class RandomizerAddSoul : FsmStateAction
    {
        private readonly GameObject _gameObject;

        public RandomizerAddSoul(GameObject baseObj)
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
            GameObject soulPrefab = ObjectCache.Soul;

            // Workaround because Spawn extension is slightly broken
            Object.Destroy(soulPrefab.Spawn());

            soulPrefab.SetActive(true);

            FlingUtils.Config flingConfig = new FlingUtils.Config
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
            HeroController.instance.StartCoroutine(SpawnSoulRoutine(transform, total, spawnsPerFrame));
        }


        private static IEnumerator SpawnSoulRoutine(Transform transform, int total, int spawnsPerFrame)
        {
            GameObject soulPrefab = ObjectCache.Soul;
            soulPrefab.GetComponent<SoulOrb>().dontRecycle = true; // can't be correctly recycled, spams nres on application quit otherwise

            // Workaround because Spawn extension is slightly broken
            Object.Destroy(soulPrefab.Spawn());

            soulPrefab.SetActive(true);

            FlingUtils.Config flingConfig = new FlingUtils.Config
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
                FlingUtils.SpawnAndFling(flingConfig, transform, new Vector3(0f, 0f, 0f));
                yield return null;
            }

            flingConfig.AmountMin = flingConfig.AmountMax = total % spawnsPerFrame;
            FlingUtils.SpawnAndFling(flingConfig, transform, new Vector3(0f, 0f, 0f));
        }
    }
}