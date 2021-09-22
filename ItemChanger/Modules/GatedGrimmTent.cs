using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Adds a gate in Grimm_Main_Tent when the Nightmare Lantern has not been lit.
    /// </summary>
    public class GatedGrimmTent : Module
    {
        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Grimm_Main_Tent, ActivateGrimmTentGate);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Grimm_Main_Tent, ActivateGrimmTentGate);
        }

        private void ActivateGrimmTentGate(Scene to)
        {
            if (!PlayerData.instance.GetBool(nameof(PlayerData.nightmareLanternLit)))
            {
                PlayMakerFSM.BroadcastEvent("BG CLOSE");
            }
        }
    }
}
