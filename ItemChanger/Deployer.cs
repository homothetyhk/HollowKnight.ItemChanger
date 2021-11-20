using ItemChanger.Internal;

namespace ItemChanger
{
    public interface IDeployer
    {
        /// <summary>
        /// Used as a key for the scene change event, and thus should not mutate.
        /// </summary>
        string SceneName { get; }
        float X { get; }
        float Y { get; }
        GameObject Deploy();
        void OnSceneChange(Scene to);
    }

    public abstract class Deployer : IDeployer
    {
        public string SceneName { get; init; }
        public float X { get; init; }
        public float Y { get; init; }
        public abstract GameObject Instantiate();

        public virtual void OnSceneChange(Scene to)
        {
            Deploy();
        }

        public virtual GameObject Deploy()
        {
            GameObject obj = Instantiate();
            obj.transform.position = new(X, Y);
            obj.SetActive(true);
            return obj;
        }
    }

    public class SmallPlatform : Deployer
    {
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
