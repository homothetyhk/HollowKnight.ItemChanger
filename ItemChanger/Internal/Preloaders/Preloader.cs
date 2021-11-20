using System.Runtime.CompilerServices;

namespace ItemChanger.Internal.Preloaders
{
    public abstract class Preloader
    {
        public PreloadLevel PreloadLevel { get; init; }
        public abstract IEnumerable<(string, string)> GetPreloadNames();
        public abstract void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene);
        protected InvalidOperationException NotPreloadedException([CallerMemberName] string caller = null)
        {
            return new($"Object {caller} was not preloaded.");
        }
    }
}
