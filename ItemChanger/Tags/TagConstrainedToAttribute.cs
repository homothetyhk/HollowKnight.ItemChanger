namespace ItemChanger.Tags
{
    /// <summary>
    /// An attribute which appears on a subclass of <see cref="Tag"/> to indicate that the tag can be placed on a certain type of <see cref="TaggableObject"/>.
    /// <br/>If the tag has multiple copies of the attribute, it can be placed on any of the specified types. The attribute is not inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class TagConstrainedToAttribute : Attribute
    {
        public abstract Type TaggableObjectType { get; }
    }
    public class TagConstrainedToAttribute<T> : TagConstrainedToAttribute where T : TaggableObject
    {
        public override Type TaggableObjectType => typeof(T);
    }
    public class ItemTagAttribute : TagConstrainedToAttribute<AbstractItem> { }
    public class LocationTagAttribute : TagConstrainedToAttribute<AbstractLocation> { }
    public class PlacementTagAttribute : TagConstrainedToAttribute<AbstractPlacement> { }
}
