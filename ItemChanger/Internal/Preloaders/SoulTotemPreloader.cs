using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ItemChanger.Internal.Preloaders
{
    public class SoulTotemPreloader : Preloader
    {
        public override IEnumerable<(string, string)> GetPreloadNames()
        {
            if (PreloadLevel != PreloadLevel.None)
            {
                yield return (SceneNames.Deepnest_East_17, "Soul Totem mini_two_horned");
            }
            if (PreloadLevel == PreloadLevel.Full)
            {
                yield return (SceneNames.Cliffs_02, "Soul Totem 5");
                yield return (SceneNames.Abyss_04, "Soul Totem mini_horned");
                yield return (SceneNames.Deepnest_10, "Soul Totem 1");
                yield return (SceneNames.RestingGrounds_05, "Soul Totem 4");
                yield return (SceneNames.Crossroads_ShamanTemple, "Soul Totem 2");
                yield return (SceneNames.Ruins1_32, "Soul Totem 3");
                yield return (SceneNames.White_Palace_02, "Soul Totem white");
                yield return (SceneNames.White_Palace_18, "Soul Totem white_Infinte");
            }
        }

        public override void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            if (PreloadLevel == PreloadLevel.Full)
            {
                _soulTotems = new()
                {
                    [SoulTotemSubtype.A] = objectsByScene[SceneNames.Cliffs_02]["Soul Totem 5"],
                    [SoulTotemSubtype.B] = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"],
                    [SoulTotemSubtype.C] = objectsByScene[SceneNames.Abyss_04]["Soul Totem mini_horned"],
                    [SoulTotemSubtype.D] = objectsByScene[SceneNames.Deepnest_10]["Soul Totem 1"],
                    [SoulTotemSubtype.E] = objectsByScene[SceneNames.RestingGrounds_05]["Soul Totem 4"],
                    [SoulTotemSubtype.F] = objectsByScene[SceneNames.Crossroads_ShamanTemple]["Soul Totem 2"],
                    [SoulTotemSubtype.G] = objectsByScene[SceneNames.Ruins1_32]["Soul Totem 3"],
                    [SoulTotemSubtype.Palace] = objectsByScene[SceneNames.White_Palace_02]["Soul Totem white"],
                    [SoulTotemSubtype.PathOfPain] = objectsByScene[SceneNames.White_Palace_18]["Soul Totem white_Infinte"]
                };
                foreach (GameObject g in _soulTotems.Values) UObject.DontDestroyOnLoad(g);
            }
            else if (PreloadLevel == PreloadLevel.Reduced)
            {
                _soulTotems = new()
                {
                    [SoulTotemSubtype.B] = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"]
                };
                foreach (GameObject g in _soulTotems.Values) UObject.DontDestroyOnLoad(g);
            }
        }

        private Dictionary<SoulTotemSubtype, GameObject> _soulTotems;

        public GameObject SoulTotem(SoulTotemSubtype t)
        {
            if (PreloadLevel == PreloadLevel.None) throw NotPreloadedException();
            return UObject.Instantiate(_soulTotems[GetPreloadedTotemType(t)]);
        }

        public SoulTotemSubtype GetPreloadedTotemType(SoulTotemSubtype t)
        {
            return _soulTotems.ContainsKey(t) ? t : SoulTotemSubtype.B;
        }
    }
}
