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
            Events.AddFsmEdit(sceneName, new("Avalanche", "Activate"), Destroy);
            Events.AddFsmEdit(sceneName, new("Avalanche End", "Control"), Destroy);
            try
            {
                Type.GetType("QoL.SettingsOverride, QoL")
                    ?.GetMethod("OverrideSettingToggle", BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { "SkipCutscenes", "AfterKingsBrandGet", false });
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Avalanche", "Activate"), Destroy);
            Events.RemoveFsmEdit(sceneName, new("Avalanche End", "Control"), Destroy);
            try
            {
                Type.GetType("QoL.SettingsOverride, QoL")
                    ?.GetMethod("RemoveSettingOverride", BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { "SkipCutscenes", "AfterKingsBrandGet" });
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void Destroy(PlayMakerFSM fsm) => UnityEngine.Object.Destroy(fsm.gameObject);
    }
}
