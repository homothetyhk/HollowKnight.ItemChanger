using Newtonsoft.Json;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Base type for classes which perform self-contained changes that should be applied when a save is created or continued and disabled when the save is unloaded.
    /// </summary>
    public abstract class Module
    {
        public string Name => GetType().Name;
        public abstract void Initialize();
        public abstract void Unload();

        /// <summary>
        /// Initializes the module and sets the Loaded property. If the module is already loaded, does nothing.
        /// <br/>Preferred to "Initialize", which does not set the Loaded property.
        /// </summary>
        public void LoadOnce()
        {
            if (!Loaded)
            {
                try
                {
                    Initialize();
                }
                catch (Exception e)
                {
                    LogError($"Error initializing module {Name}:\n{e}");
                }
                Loaded = true;
            }
        }

        /// <summary>
        /// Unloads the module and sets the Loaded property. If the module is not loaded, does nothing.
        /// <br/>Preferred to "Unload", which does not set the Loaded property.
        /// </summary>
        public void UnloadOnce()
        {
            if (Loaded)
            {
                try
                {
                    Unload();
                }
                catch (Exception e)
                {
                    LogError($"Error unloading module {Name}:\n{e}");
                }
                Loaded = false;
            }
        }

        /// <summary>
        /// Additional information for serialization and other tag handling purposes.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual ModuleHandlingFlags ModuleHandlingProperties { get; set; }

        [JsonIgnore]
        public bool Loaded { get; private set; }
    }

    /// <summary>
    /// Attribute which marks that a module should be included automatically in a new save. This functionality only applies to types declared in the ItemChangerMod assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultModuleAttribute : Attribute { }

}
