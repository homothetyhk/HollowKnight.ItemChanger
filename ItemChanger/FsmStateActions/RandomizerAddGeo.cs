using HutongGames.PlayMaker;
using SereCore;
using UnityEngine;
using Random = System.Random;

namespace ItemChanger.FsmStateActions
{
    internal class RandomizerAddGeo : FsmStateAction
    {
        private const int GEO_VALUE_LARGE = 25;
        private const int GEO_VALUE_MEDIUM = 5;

        private readonly GameObject _gameObject;
        private readonly bool _minimize;
        private int _count;

        public RandomizerAddGeo(GameObject baseObj, int amount, bool minimizeObjects = false)
        {
            _count = amount;
            _gameObject = baseObj;
            _minimize = minimizeObjects;
        }

        public void SetGeo(int geo)
        {
            _count = geo;
        }

        public override void OnEnter()
        {
            // Special case for pickups where you don't have an opportunity to pick up the geo
            string sceneName = Ref.GM.GetSceneNameString();
            if (sceneName == SceneNames.Dream_Nailcollection || sceneName == SceneNames.Room_Sly_Storeroom || sceneName == SceneNames.Abyss_08)
            {
                Ref.Hero.AddGeo(_count);
                Finish();
                return;
            }

            int smallNum;
            int medNum;
            int largeNum;

            if (!_minimize)
            {
                Random random = new Random();

                smallNum = random.Next(0, _count / 10);
                _count -= smallNum;
                largeNum = random.Next(_count / (GEO_VALUE_LARGE * 2), _count / GEO_VALUE_LARGE + 1);
                _count -= largeNum * GEO_VALUE_LARGE;
                medNum = _count / GEO_VALUE_MEDIUM;
                _count -= medNum * 5;
                smallNum += _count;
            }
            else
            {
                largeNum = _count / GEO_VALUE_LARGE;
                _count -= largeNum * GEO_VALUE_LARGE;
                medNum = _count / GEO_VALUE_MEDIUM;
                _count -= medNum * GEO_VALUE_MEDIUM;
                smallNum = _count;
            }

            GameObject smallPrefab = ObjectCache.SmallGeo;
            GameObject mediumPrefab = ObjectCache.MediumGeo;
            GameObject largePrefab = ObjectCache.LargeGeo;

            // Workaround because Spawn extension is slightly broken
            Object.Destroy(smallPrefab.Spawn());
            Object.Destroy(mediumPrefab.Spawn());
            Object.Destroy(largePrefab.Spawn());

            smallPrefab.SetActive(true);
            mediumPrefab.SetActive(true);
            largePrefab.SetActive(true);

            FlingUtils.Config flingConfig = new FlingUtils.Config
            {
                Prefab = smallPrefab,
                AmountMin = smallNum,
                AmountMax = smallNum,
                SpeedMin = 15f,
                SpeedMax = 30f,
                AngleMin = 80f,
                AngleMax = 115f
            };

            // Special case for thorns of agony, spore shroom, flukenest to stop geo from flying into unreachable spots
            if (sceneName == SceneNames.Fungus1_14 || sceneName == SceneNames.Fungus2_20 ||
                sceneName == SceneNames.Waterways_12)
            {
                flingConfig.AngleMin = 90;
                flingConfig.AngleMax = 90;
            }

            if (smallNum > 0)
            {
                FlingUtils.SpawnAndFling(flingConfig, _gameObject.transform, new Vector3(0f, 0f, 0f));
            }

            if (medNum > 0)
            {
                flingConfig.Prefab = mediumPrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = medNum;
                FlingUtils.SpawnAndFling(flingConfig, _gameObject.transform, new Vector3(0f, 0f, 0f));
            }

            if (largeNum > 0)
            {
                flingConfig.Prefab = largePrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = largeNum;
                FlingUtils.SpawnAndFling(flingConfig, _gameObject.transform, new Vector3(0f, 0f, 0f));
            }

            smallPrefab.SetActive(false);
            mediumPrefab.SetActive(false);
            largePrefab.SetActive(false);

            Finish();
        }
    }
}