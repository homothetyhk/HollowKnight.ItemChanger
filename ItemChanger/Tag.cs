namespace ItemChanger
{
    public abstract class Tag
    {
        /// <summary>
        /// Virtual method called on tags when their parent loads. The base method checks and throws an exception if the tag is already loaded.
        /// <br/>This should not be called directly. Instead, use "LoadOnce" to load and set the Loaded property.
        /// </summary>
        /// <exception cref="InvalidOperationException">The tag is already loaded.</exception>
        public virtual void Load(object parent) 
        {
            if (Loaded) throw new InvalidOperationException($"Tag {GetType().Name} is already loaded.");
        }
        /// <summary>
        /// Virtual method called on tags when their parent unloads. The base method checks and throws an exception if the tag is not loaded.
        /// <br/>This should not be called directly. Instead, use "UnloadOnce" to unload and set the Loaded property.
        /// </summary>
        /// <exception cref="InvalidOperationException">The tag is not loaded.</exception>
        public virtual void Unload(object parent) 
        {
            if (!Loaded) throw new InvalidOperationException($"Tag {GetType().Name} was not loaded.");
        }
        public virtual Tag Clone() => (Tag)MemberwiseClone();

        /// <summary>
        /// Loads the tag and sets the Loaded property. If the tag is already loaded, does nothing.
        /// <br/>Preferred to "Load", which does not set the Loaded property.
        /// </summary>
        public void LoadOnce(TaggableObject parent)
        {
            if (!Loaded)
            {
                try
                {
                    Load(parent);
                }
                catch (Exception e)
                {
                    LogError($"Error loading {GetType().Name}:\n{e}");
                }
                Loaded = true;
            }
        }

        /// <summary>
        /// Unloads the tag and sets the Loaded property. If the tag is not loaded, does nothing.
        /// <br/>Preferred to "Unload", which does not set the Loaded property.
        /// </summary>
        public void UnloadOnce(TaggableObject parent)
        {
            if (Loaded)
            {
                try
                {
                    Unload(parent);
                }
                catch (Exception e)
                {
                    LogError($"Error unloading {GetType().Name}:\n{e}");
                }
                Loaded = false;
            }
        }

        public bool Loaded { get; private set; }
    }
}
