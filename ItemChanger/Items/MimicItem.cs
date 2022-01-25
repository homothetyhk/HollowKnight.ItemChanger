using ItemChanger.Internal;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which plays the MimicScream clip if the placement did not use a mimic container.
    /// </summary>
    public class MimicItem : AbstractItem
    {
        public override string GetPreferredContainer()
        {
            return Container.Mimic;
        }

        public override bool GiveEarly(string containerType)
        {
            return containerType == Container.Mimic;
        }

        public override void GiveImmediate(GiveInfo info)
        {
            if (info.Container != Container.Mimic)
            {
                SoundManager.Instance.PlayClipAtPoint("MimicScream",
                info.Transform != null ? info.Transform.position
                : HeroController.instance != null ? HeroController.instance.transform.position
                : Camera.main.transform.position + 2 * Vector3.up);
            }
        }
    }
}
