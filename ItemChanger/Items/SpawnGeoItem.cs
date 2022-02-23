namespace ItemChanger.Items
{
    /// <summary>
    /// Item which spawns a specified amount of geo.
    /// </summary>
    public class SpawnGeoItem : AbstractItem
    {
        public static SpawnGeoItem MakeGeoItem(int amount)
        {
            return new()
            {
                name = $"{amount}_Geo",
                amount = amount,
                UIDef = new UIDefs.MsgUIDef
                {
                    name = new BoxedString(string.Format(Language.Language.Get("GEO", "Fmt"), amount)),
                    shopDesc = new LanguageString("UI", "ITEMCHANGER_DESC_GEO"),
                    sprite = new ItemChangerSprite("ShopIcons.Geo"),
                },
            };
        }

        public int amount;

        public override bool GiveEarly(string containerType)
        {
            return containerType switch
            {
                Container.Enemy 
                or Container.Chest 
                or Container.GeoRock 
                or Container.GrubJar 
                or Container.Mimic
                  => true,
                _ => false,
            };
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.FlingType == FlingType.DirectDeposit || info.Transform == null)
            {
                if (HeroController.SilentInstance != null && HeroController.SilentInstance.geoCounter != null)
                {
                    HeroController.SilentInstance.AddGeo(amount);
                }
                else
                {
                    PlayerData.instance.AddGeo(amount);
                }
                return;
            }
            FsmStateActions.FlingGeoAction.SpawnGeo(amount, false, info.FlingType, info.Transform);
        }
    }
}
