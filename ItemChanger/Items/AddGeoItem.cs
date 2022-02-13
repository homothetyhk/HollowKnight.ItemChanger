namespace ItemChanger.Items
{
    /// <summary>
    /// Item which directly adds geo to the inventory.
    /// </summary>
    public class AddGeoItem : AbstractItem
    {
        public int amount;

        public override void GiveImmediate(GiveInfo info)
        {
            if (HeroController.SilentInstance != null && HeroController.SilentInstance.geoCounter != null)
            {
                HeroController.instance.AddGeo(amount);
            }
            else
            {
                PlayerData.instance.AddGeo(amount);
            }
        }
    }
}
