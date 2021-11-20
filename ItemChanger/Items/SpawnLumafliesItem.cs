namespace ItemChanger.Items
{
    /// <summary>
    /// Item which spawns escaped lumaflies.
    /// </summary>
    public class SpawnLumafliesItem : AbstractItem
    {
        public override bool GiveEarly(string containerType) => containerType switch
        {
            Container.Chest 
            or Container.GrubJar
            or Container.GeoRock
            or Container.Enemy
            or Container.Mimic 
              => true,
            _ => false
        };

        public override void GiveImmediate(GiveInfo info)
        {
            Transform t = info.Transform != null ? info.Transform : HeroController.instance.transform;
            if (t == null) return;

            GameObject lumafly = Internal.ObjectCache.LumaflyEscape;
            lumafly.transform.position = new(t.position.x, t.position.y, t.position.z - 5);
            lumafly.SetActive(true);
        }
    }
}
