using Modding;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// Boss Essence location which applies to dream bosses and supports respawning items.
    /// </summary>
    public class DreamBossLocation : BossEssenceLocation
    {
        public string playerDataBool;
        protected override void OnLoad()
        {
            base.OnLoad();
            ModHooks.GetPlayerBoolHook += OverrideCollectedBool;
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            ModHooks.GetPlayerBoolHook -= OverrideCollectedBool;
        }

        private bool OverrideCollectedBool(string name, bool orig)
        {
            if (name == playerDataBool && orig && !Placement.AllObtained())
            {
                return false;
            }
            return orig;
        }
    }
}
