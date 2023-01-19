using ItemChanger.Components;

namespace ItemChanger
{
    /// <summary>
    /// Data for instructing a Container class to make changes. The ContainerGiveInfo field must not be null.
    /// </summary>
    public class ContainerInfo
    {
        public string containerType;

        public ContainerGiveInfo giveInfo;
        public ChangeSceneInfo? changeSceneInfo;
        public CostInfo? costInfo;

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, FlingType flingType) 
            : this(containerType, placement, placement.Items, flingType)
        {
        }

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType) 
            : this()
        {
            this.containerType = containerType;
            this.giveInfo = new()
            {
                placement = placement,
                items = items,
                flingType = flingType,
            };
        }

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo. If the cost parameter is not null, initializes costInfo with the cost.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, FlingType flingType, Cost? cost) 
            : this(containerType, placement, placement.Items, flingType, cost)
        {
        }

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo. If the cost parameter is not null, initializes costInfo with the cost.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost? cost) 
            : this(containerType, placement, items, flingType)
        {
            if (cost is not null)
            {
                this.costInfo = new()
                {
                    placement = placement,
                    cost = cost,
                    previewItems = items,
                };
            }
        }

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo and the provided ChangeSceneInfo. If the cost parameter is not null, initializes costInfo with the cost.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, FlingType flingType, Cost? cost, ChangeSceneInfo? changeSceneInfo) 
            : this(containerType, placement, placement.Items, flingType, cost, changeSceneInfo)
        {
        }

        /// <summary>
        /// Creates ContainerInfo with standard ContainerGiveInfo and the provided ChangeSceneInfo. If the cost parameter is not null, initializes costInfo with the cost.
        /// </summary>
        public ContainerInfo(string containerType, AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost? cost, ChangeSceneInfo? changeSceneInfo)
            : this(containerType, placement, items, flingType, cost)
        {
            this.changeSceneInfo = changeSceneInfo;
        }

        public ContainerInfo(string containerType, ContainerGiveInfo giveInfo, CostInfo? costInfo, ChangeSceneInfo? changeSceneInfo)
        {
            this.containerType = containerType;
            this.giveInfo = giveInfo;
            this.costInfo = costInfo;
            this.changeSceneInfo = changeSceneInfo;
        }


        /// <summary>
        /// Creates uninitialized ContainerInfo. The giveInfo and containerType fields must be initialized before use.
        /// </summary>
        public ContainerInfo()
        {
        }

        /// <summary>
        /// Searches for ContainerInfo on a ContainerInfoComponent. Returns null if neither is found.
        /// </summary>
        public static ContainerInfo? FindContainerInfo(GameObject obj)
        {
            var cdc = obj.GetComponent<ContainerInfoComponent>();
            if (cdc != null) return cdc.info;
            return null;
        }
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
        public bool dreamReturn;
        public bool deactivateNoCharms;

        public bool applied;

        public ChangeSceneInfo() { }
        public ChangeSceneInfo(Tags.ChangeSceneTag cst)
        {
            transition = cst.changeTo;
            dreamReturn = cst.dreamReturn;
            deactivateNoCharms = cst.deactivateNoCharms;
        }
        public ChangeSceneInfo(Transition transition)
        {
            this.transition = transition;
            this.dreamReturn = this.deactivateNoCharms = transition.GateName == door_dreamReturn;
        }
        public ChangeSceneInfo(Transition transition, bool dreamReturn)
        {
            this.transition = transition;
            this.dreamReturn = this.deactivateNoCharms = dreamReturn;
        }
        public ChangeSceneInfo(Transition transition, bool dreamReturn, bool deactivateNoCharms)
        {
            this.transition = transition;
            this.dreamReturn = dreamReturn;
            this.deactivateNoCharms = deactivateNoCharms;
        }
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
