using static ItemChanger.ChangeSceneInfo;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which certain placements or locations may use to add a scene change after obtaining items.
    /// </summary>
    [LocationTag]
    [PlacementTag]
    public class ChangeSceneTag : Tag
    {
        public Transition changeTo;
        public bool dreamReturn = true;
        public bool deactivateNoCharms = true;

        public ChangeSceneTag() { }

        public ChangeSceneTag(Transition changeTo)
        {
            this.changeTo = changeTo;
            this.dreamReturn = this.deactivateNoCharms = changeTo.GateName == door_dreamReturn;
        }

        public ChangeSceneTag(Transition changeTo, bool dreamReturn)
        {
            this.changeTo = changeTo;
            this.dreamReturn = this.deactivateNoCharms = dreamReturn;
        }

        public ChangeSceneTag(Transition changeTo, bool dreamReturn, bool deactivateNoCharms)
        {
            this.changeTo = changeTo;
            this.dreamReturn = dreamReturn;
            this.deactivateNoCharms = deactivateNoCharms;
        }

        public ChangeSceneInfo ToChangeSceneInfo()
        {
            return new(this);
        }
    }
}
