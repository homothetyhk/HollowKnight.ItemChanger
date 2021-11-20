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
    }

    /// <summary>
    /// Attribute which marks that a module should be included automatically in a new save. This functionality only applies to types declared in the ItemChangerMod assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultModuleAttribute : Attribute { }

}
