namespace ItemChanger
{
    /// <summary>
    /// Interface for very simple scene change actions, which can be added directly to settings.
    /// <br/>The most common use is a Deployer, which creates an object at specified coordinates.
    /// </summary>
    public interface IDeployer
    {
        /// <summary>
        /// The scene in which the IDeployer acts. Used as a key for the scene change event, and thus should not mutate.
        /// </summary>
        string SceneName { get; }
        /// <summary>
        /// Action to perform when SceneName becomes the active scene.
        /// </summary>
        void OnSceneChange(Scene to);
    }
}
