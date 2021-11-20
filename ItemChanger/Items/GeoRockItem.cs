namespace ItemChanger.Items
{
    /// <summary>
    /// Item which spawns the specified amount of geo, and requests a geo rock container of the specified GeoRockSubtype.
    /// </summary>
    public class GeoRockItem : AbstractItem
    {
        public override string GetPreferredContainer() => Container.GeoRock;
        public override bool GiveEarly(string containerType)
        {
            return containerType switch
            {
                // Container.Enemy // Not included, so that the geo rock spawns on death!
                Container.Chest 
                or Container.GeoRock 
                or Container.GrubJar 
                or Container.Mimic
                  => true,
                _ => false,
            };
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

            FsmStateActions.FlingGeoAction.SpawnGeo(smallNum, medNum, largeNum, info.FlingType, info.Transform);
        }
    }
}
