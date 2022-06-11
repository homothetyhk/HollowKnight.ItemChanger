using ItemChanger.Internal;

namespace ItemChanger
{
    /// <summary>
    /// A Deployer which creates a platform at the specified point.
    /// </summary>
    public record SmallPlatform : Deployer
    {
        /// <summary>
        /// Optional property. If non-null and evaluates false, the platform is not deployed.
        /// </summary>
        public IBool Test { get; init; } = null;

        public override GameObject Instantiate()
        {
            return ObjectCache.SmallPlatform;
        }

        public override void OnSceneChange(Scene to)
        {
            if (Test != null && !Test.Value) return;
            else base.OnSceneChange(to);
        }
    }
}
