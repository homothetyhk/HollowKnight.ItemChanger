using System.Reflection;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which prevents the King's Brand avalanche sequence from occuring.
    /// </summary>
    public class KingsBrandLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(UnsafeSceneName, new("Avalanche", "Activate"), Destroy);
            Events.AddFsmEdit(UnsafeSceneName, new("Avalanche End", "Control"), Destroy);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(UnsafeSceneName, new("Avalanche", "Activate"), Destroy);
            Events.RemoveFsmEdit(UnsafeSceneName, new("Avalanche End", "Control"), Destroy);
        }

        private void Destroy(PlayMakerFSM fsm) => UnityEngine.Object.Destroy(fsm.gameObject);
    }
}
