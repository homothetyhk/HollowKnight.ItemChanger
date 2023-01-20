using System.Reflection;

namespace ItemChanger.Tags
{
    /// <summary>
    /// A tag which enables a QoL override for an individual setting in a module, or an entire module.
    /// </summary>
    public class QolOverrideTag : Tag
    {
        public string ModuleName;
        public string? SettingName;
        public bool Enable;

        public override void Load(object parent)
        {
            base.Load(parent);

            try
            {
                Type settingsOverride = Type.GetType("QoL.SettingsOverride, QoL");
                if (SettingName == null)
                {
                    settingsOverride
                        ?.GetMethod("OverrideModuleToggle", BindingFlags.Public | BindingFlags.Static)
                        .Invoke(null, new object[] { ModuleName, Enable });
                }
                else
                {
                    settingsOverride
                        ?.GetMethod("OverrideSettingToggle", BindingFlags.Public | BindingFlags.Static)
                        .Invoke(null, new object[] { ModuleName, SettingName, Enable });
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public override void Unload(object parent)
        {
            base.Unload(parent);

            try
            {
                Type settingsOverride = Type.GetType("QoL.SettingsOverride, QoL");
                if (SettingName == null)
                {
                    settingsOverride
                        ?.GetMethod("RemoveModuleOverride", BindingFlags.Public | BindingFlags.Static)
                        ?.Invoke(null, new object[] { ModuleName });
                }
                else
                {
                    settingsOverride
                        ?.GetMethod("RemoveSettingOverride", BindingFlags.Public | BindingFlags.Static)
                        ?.Invoke(null, new object[] { ModuleName, SettingName });
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
    }
}
