namespace ItemChanger.Components
{
    /// <summary>
    /// Component to be attached to a container GameObject to allow the Container class to make changes.
    /// </summary>
    public class ContainerInfo : MonoBehaviour
    {
        public string containerType;

        public ContainerGiveInfo giveInfo;
        public ChangeSceneInfo changeSceneInfo;
        public CostInfo costInfo;
    }

    /// <summary>
    /// Instructions for a container to give items.
    /// </summary>
    public class ContainerGiveInfo
    {
        public IEnumerable<AbstractItem> items;
        public AbstractPlacement placement;
        public FlingType flingType;
        public bool applied;
    }

    /// <summary>
    /// Instructions for a container to change scene.
    /// </summary>
    public class ChangeSceneInfo
    {
        public const string door_dreamReturn = "door_dreamReturn";

        public Transition transition;
        public bool applied;
    }

    /// <summary>
    /// Instructions for a container to enforce a Cost.
    /// </summary>
    public class CostInfo
    {
        public Cost cost;
        public IEnumerable<AbstractItem> previewItems;
        public AbstractPlacement placement;
        public bool applied;
    }
}
