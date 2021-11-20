using Newtonsoft.Json;

namespace ItemChanger
{
    /// <summary>
    /// The base class for all locations. Locations are used by placements to place items.
    /// <br/>Usually the location contains raw data and an implementation that may be customizable to an extent by the placement.
    /// </summary>
    public abstract class AbstractLocation : TaggableObject
    {
        /// <summary>
        /// The name of the location. Location names are often, but not always, distinct.
        /// </summary>
        public string name;

        /// <summary>
        /// The scene name of the location. Locations can however make changes which affect more than one scene, and rarely may choose not to use this field.
        /// </summary>
        public string sceneName;

        /// <summary>
        /// The flingType of the location, specifying how geo and similar objects are to be flung.
        /// </summary>
        public FlingType flingType;

        /// <summary>
        /// The placement holding the location. This is usually set by the placement when the placement loads and before the location loads.
        /// </summary>
        [JsonIgnore]
        public AbstractPlacement Placement { get; set; }

        /// <summary>
        /// Called on a location by its placement, usually during AbstractPlacement.Load().
        /// <br/>Execution order is (modules load -> placement tags load -> items load -> placements load)
        /// </summary>
        public void Load()
        {
            LoadTags();
            OnLoad();
        }

        /// <summary>
        /// Called on a location by its placement, usually during AbstractPlacement.Unload().
        /// <br/>Execution order is (modules unload -> placement tags unload -> items unload -> placements unload)
        /// </summary>
        public void Unload()
        {
            UnloadTags();
            OnUnload();
        }

        /// <summary>
        /// Called during Load(). Allows the location to initialize and set up any hooks.
        /// </summary>
        protected abstract void OnLoad();

        /// <summary>
        /// Called during Unload(). Allows the location to dispose any hooks.
        /// </summary>
        protected abstract void OnUnload();

        /// <summary>
        /// Creates a default placement for this location.
        /// </summary>
        public abstract AbstractPlacement Wrap();

        /// <summary>
        /// Performs a deep clone of the location.
        /// </summary>
        public virtual AbstractLocation Clone()
        {
            AbstractLocation location = (AbstractLocation)MemberwiseClone();
            location.tags = location.tags?.Select(t => t.Clone())?.ToList();
            return location;
        }
    }
}
