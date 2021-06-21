using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Items
{
    public class GeoRockItem : AbstractItem
    {
        public override Container GetPreferredContainer() => Container.GeoRock;
        public override bool GiveEarly(Container container)
        {
            switch (container)
            {
                case Container.Chest:
                case Container.GeoRock:
                case Container.GrubJar:
                    return true;
                default:
                    return false;
            }
        }

        public GeoRockSubtype geoRockSubtype;
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.FlingType == FlingType.DirectDeposit || info.Transform == null)
            {
                if (HeroController.instance != null)
                {
                    HeroController.instance.AddGeo(amount);
                }
                else
                {
                    PlayerData.instance.AddGeo(amount);
                }
                return;
            }

            int smallNum;
            int medNum;
            int largeNum;

            if (amount < 70)
            {
                smallNum = amount;
                medNum = 0;
                largeNum = 0;
            }
            else if (amount < 425)
            {
                medNum = amount / 5;
                smallNum = amount - 5 * medNum;
                largeNum = 0;
            }
            else
            {
                largeNum = amount / 25;
                medNum = (amount - largeNum * 25) / 5;
                smallNum = amount - largeNum * 25 - medNum * 5;
            }

            GameObject smallPrefab = ObjectCache.SmallGeo;
            GameObject mediumPrefab = ObjectCache.MediumGeo;
            GameObject largePrefab = ObjectCache.LargeGeo;

            // Workaround because Spawn extension is slightly broken
            GameObject.Destroy(smallPrefab.Spawn());
            GameObject.Destroy(mediumPrefab.Spawn());
            GameObject.Destroy(largePrefab.Spawn());

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

            if (smallNum > 0)
            {
                FlingUtils.SpawnAndFling(flingConfig, info.Transform, new Vector3(0f, 0f, 0f));
            }

            if (medNum > 0)
            {
                flingConfig.Prefab = mediumPrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = medNum;
                FlingUtils.SpawnAndFling(flingConfig, info.Transform, new Vector3(0f, 0f, 0f));
            }

            if (largeNum > 0)
            {
                flingConfig.Prefab = largePrefab;
                flingConfig.AmountMin = flingConfig.AmountMax = largeNum;
                FlingUtils.SpawnAndFling(flingConfig, info.Transform, new Vector3(0f, 0f, 0f));
            }

            if (info.FlingType == FlingType.StraightUp)
            {
                flingConfig.AngleMin = 90;
                flingConfig.AngleMax = 90;
            }

            smallPrefab.SetActive(false);
            mediumPrefab.SetActive(false);
            largePrefab.SetActive(false);
        }
    }
}
