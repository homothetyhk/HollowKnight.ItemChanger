using Modding;
using ItemChanger.Locations.SpecialLocations;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Nondefault module added by a NailmasterLocation in Room_Nailmaster_02. Ties PD.nailsmithSheo to having accepted Sheo's offer.
    /// </summary>
    public class AltNailsmithSheoTest : Module
    {
        private readonly List<NailmasterLocation> nailmasters = new();

        public override void Initialize()
        {
            ModHooks.GetPlayerBoolHook += GetNailsmithSheo;
        }

        public override void Unload()
        {
            ModHooks.GetPlayerBoolHook -= GetNailsmithSheo;
        }

        private bool GetNailsmithSheo(string name, bool orig) => name switch
        {
            nameof(PlayerData.nailsmithSheo) => PlayerData.instance.GetBool(nameof(PlayerData.nailsmithSpared)) && nailmasters.All(nm => nm.AcceptedOffer()),
            _ => orig
        };

        public void Subscribe(NailmasterLocation location) => nailmasters.Add(location);
    }
}
