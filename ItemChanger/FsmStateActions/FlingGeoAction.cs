using HutongGames.PlayMaker;
using ItemChanger.Internal;
using ItemChanger.Extensions;
using UnityEngine;
using Random = System.Random;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction with static methods for flinging geo from a transform.
    /// </summary>
    public class FlingGeoAction : FsmStateAction
    {
        private const int GEO_VALUE_LARGE = 25;
        private const int GEO_VALUE_MEDIUM = 5;
        

        private readonly GameObject _gameObject;
        private readonly bool _minimize;
        private FsmInt _amount;

        public FlingGeoAction(GameObject baseObj, FsmInt amount, bool minimizeObjects = false)
        {
            _amount = amount;
            _gameObject = baseObj;
            _minimize = minimizeObjects;
        }

        public static void SpawnGeo(int _count, bool _minimize, FlingType fling, Transform _transform, bool normalizeZ = true)
        {
            int smallNum;
            int medNum;
            int largeNum;

            if (!_minimize)
            {
                Random random = new();

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

            SpawnGeo(smallNum, medNum, largeNum, fling, _transform, normalizeZ);
        }

        public static void SpawnGeo(int smallNum, int medNum, int largeNum, FlingType fling, Transform transform, bool normalizeZ = true)
        {
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

            FlingUtils.Config flingConfig = new()
            {
                SpeedMin = 15f,
                SpeedMax = 30f,
                AngleMin = 80f,
                AngleMax = 115f
            };

            Vector3 offset = normalizeZ ? Vector3.back * transform.position.z : Vector3.zero;

            if (fling == FlingType.StraightUp)
            {
                flingConfig.AngleMin = 90;
                flingConfig.AngleMax = 90;
            }

            if (smallNum > 0)
            {
                flingConfig.Prefab = smallPrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = smallNum;
                FlingUtils.SpawnAndFling(flingConfig, transform, offset);
            }

            if (medNum > 0)
            {
                flingConfig.Prefab = mediumPrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = medNum;
                FlingUtils.SpawnAndFling(flingConfig, transform, offset);
            }

            if (largeNum > 0)
            {
                flingConfig.Prefab = largePrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = largeNum;
                FlingUtils.SpawnAndFling(flingConfig, transform, offset);
            }

            smallPrefab.SetActive(false);
            mediumPrefab.SetActive(false);
            largePrefab.SetActive(false);
        }


        public override void OnEnter()
        {
            // Special case for pickups where you don't have an opportunity to pick up the geo
            string sceneName = GameManager.instance.GetSceneNameString();
            int _count = _amount.Value;

            if (sceneName == SceneNames.Dream_Nailcollection || sceneName == SceneNames.Room_Sly_Storeroom || sceneName == SceneNames.Abyss_08)
            {
                HeroController.instance.AddGeo(_count);
                Finish();
                return;
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

            int smallNum;
            int medNum;
            int largeNum;

            if (!_minimize)
            {
                Random random = new();

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

            FlingUtils.Config flingConfig = new()
            {
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
                flingConfig.Prefab = smallPrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = smallNum;
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