using ItemChanger.Internal;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives a grub and requests a Grub Jar container.
    /// </summary>
    public class GrubItem : AbstractItem
    {
        public override string GetPreferredContainer() => Container.GrubJar;
        public override bool GiveEarly(string containerType)
        {
            return containerType == Container.GrubJar;
        }
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.IncrementInt(nameof(PlayerData.grubsCollected));
            if (info.Container == Container.GrubJar) return;

            SoundManager.Instance.PlayClipAtPoint(new System.Random().Next(2) == 0 ? "GrubCry0" : "GrubCry1", 
                info.Transform != null ? info.Transform.position
                : HeroController.instance != null ? HeroController.instance.transform.position
                : Camera.main.transform.position + 2 * Vector3.up);
        }
    }
}
