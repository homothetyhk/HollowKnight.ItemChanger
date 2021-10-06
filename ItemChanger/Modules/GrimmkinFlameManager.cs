using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class GrimmkinFlameManager : Module
    {
        public override void Initialize()
        {
            ModHooks.GetPlayerIntHook += OverrideGetFlamesCollected;
            On.PlayerData.SetInt += OverrideSetFlamesCollected;
        }

        public override void Unload()
        {
            ModHooks.GetPlayerIntHook -= OverrideGetFlamesCollected;
            On.PlayerData.SetInt -= OverrideSetFlamesCollected;
        }

        // When upgrading Grimmchild, Grimm sets the flame counter to 0. If there are excess flames,
        // this is wrong; we want those flames to carry over to the next level.
        // To avoid conflicts with other mods, we hook PlayerData.SetInt directly rather than
        // use SetPlayerIntHook; when using the latter, other mods using that hook, such as
        // PlayerDataTracker, will inadvertently overwrite our changes if their hook runs after ours,
        // since they only see the value the game originally tried to set and SetPlayerIntHook
        // requires the hook to write the new value itself even if it doesn't want to override it.
        private void OverrideSetFlamesCollected(On.PlayerData.orig_SetInt orig, PlayerData pd, string intName, int newValue)
        {
            switch (intName)
            {
                case nameof(PlayerData.flamesCollected) when newValue == 0:
                    newValue = pd.GetIntInternal(nameof(PlayerData.flamesCollected)) - 3;
                    break;
            }
            orig(pd, intName, newValue);
        }

        // Grimm only appears in his tent if the player has exactly 3 flames. Hide any excess
        // flames (which can only happen when flames are randomized) from the game.
        // Increments of the variable (collecting flames) will still increment the real value.
        private int OverrideGetFlamesCollected(string name, int orig) => name switch
        {
            nameof(PlayerData.flamesCollected) => Math.Min(PlayerData.instance.GetIntInternal(nameof(PlayerData.flamesCollected)), 3),
            _ => orig,
        };
    }
}
