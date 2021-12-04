using Modding;

namespace ItemChanger.Tags
{
    public class OverridePDBoolTag : Tag
    {
        public string boolName;
        public IBool value;
        public bool overrideSet;

        public override void Load(object parent)
        {
            ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
            if (overrideSet) ModHooks.SetPlayerBoolHook += SetPlayerBoolHook;
        }

        public override void Unload(object parent)
        {
            ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
            if (overrideSet) ModHooks.SetPlayerBoolHook -= SetPlayerBoolHook;
        }

        private bool GetPlayerBoolHook(string name, bool orig)
        {
            if (name == boolName) return value.Value;
            else return orig;
        }

        private bool SetPlayerBoolHook(string name, bool orig)
        {
            if (name == boolName) ((IWritableBool)value).Value = orig;
            return orig;
        }
    }
}
