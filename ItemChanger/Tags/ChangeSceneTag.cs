using ItemChanger.Components;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which certain placements or locations may use to add a scene change after obtaining items.
    /// </summary>
    public class ChangeSceneTag : Tag
    {
        public Transition changeTo;
        public bool dreamReturn;
        public bool deactivateNoCharms;

        public ChangeSceneInfo ToChangeSceneInfo()
        {
            return new(this);
        }
    }
}
