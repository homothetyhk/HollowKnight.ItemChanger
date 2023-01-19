namespace ItemChanger.Modules
{
    /// <summary>
    /// An interface implemented by modules for sharing information between assemblies that do not strongly reference each other.
    /// </summary>
    public interface IInteropModule
    {
        /// <summary>
        /// A description of the module that can be recognized by consumers.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Returns true if the property name corresponds to a non-null value of the specified type, and outputs the casted value.
        /// </summary>
        bool TryGetProperty<T>(string propertyName, out T? value);
    }

    /// <summary>
    /// Module which provides the default implementation of IInteropModule.
    /// </summary>
    public class InteropModule : Module, IInteropModule
    {
        public string Message { get; set; }
        public Dictionary<string, object?> Properties = new();

        public bool TryGetProperty<T>(string propertyName, out T? value)
        {
            if (propertyName == null || Properties == null || !Properties.TryGetValue(propertyName, out object? val) || val is not T t)
            {
                value = default;
                return false;
            }

            value = t;
            return true;
        }

        public override void Initialize() { }
        public override void Unload() { }
    }
}