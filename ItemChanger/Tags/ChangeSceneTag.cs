namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which certain placements or locations may use to add a scene change after obtaining items.
    /// </summary>
    public class ChangeSceneTag : Tag
    {
        public Transition changeTo;
    }
}
