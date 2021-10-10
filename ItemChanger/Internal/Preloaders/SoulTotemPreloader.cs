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
            yield return (SceneNames.Deepnest_East_17, "Soul Totem mini_two_horned");
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
            }
            else
            {
                _soulTotems = new()
                {
                    [SoulTotemSubtype.B] = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"]
                };
            }

            foreach (GameObject g in _soulTotems.Values) UObject.DontDestroyOnLoad(g);

            _soul = UObject.Instantiate(objectsByScene[SceneNames.Cliffs_02]["Soul Totem 5"]
                .LocateMyFSM("soul_totem").GetState("Hit").GetFirstActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value);
            _soul.GetComponent<AudioSource>().priority = 200;
            // priority is from 0 to 255
            // almost all sources (including soul) default to 128
            // if number of sources playing exceeds limit, larger priority is culled first
            // if we spawn several soul orbs at once and they have default priority, more important sounds could be culled
            // so we raise the priority on each soul orb
            _soul.SetActive(false);
            UObject.DontDestroyOnLoad(_soul);
        }

        private GameObject _soul;
        private Dictionary<SoulTotemSubtype, GameObject> _soulTotems;

        public GameObject Soul
        {
            get
            {
                return _soul; // currently always preloaded
            }
        }

        public GameObject SoulTotem(SoulTotemSubtype t)
        {
            return UObject.Instantiate(_soulTotems[GetPreloadedTotemType(t)]); // currently a totem is always preloaded, so no exception check is needed
        }

        public SoulTotemSubtype GetPreloadedTotemType(SoulTotemSubtype t)
        {
            return _soulTotems.ContainsKey(t) ? t : SoulTotemSubtype.B;
        }
    }
}
