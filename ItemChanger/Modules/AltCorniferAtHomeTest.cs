using ItemChanger.Locations.SpecialLocations;
using Modding;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Nondefault module added automatically by CorniferLocations. Ties CorniferAtHome to all CorniferLocations having been purchased.
    /// </summary>
    public class AltCorniferAtHomeTest : Module
    {
        private readonly List<CorniferLocation> cornifers = new();

        public override void Initialize()
        {
            ModHooks.GetPlayerBoolHook += GetCorniferAtHome;
        }

        public override void Unload()
        {
            ModHooks.GetPlayerBoolHook -= GetCorniferAtHome;
        }

        private bool GetCorniferAtHome(string name, bool orig) => name switch
        {
            nameof(PlayerData.corniferAtHome) => cornifers.All(c => !c.IsCorniferPresent()),
            _ => orig
        };

        public void Add(CorniferLocation location) => cornifers.Add(location);
    }
}
