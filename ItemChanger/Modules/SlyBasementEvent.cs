using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class SlyBasementEvent : Module
    {
        /// <summary>
        /// If true, Sly's basement will no longer be available.
        /// </summary>
        public bool Closed = false;

        private const float closedOffset = -1.5f;
        private const float openOffset = 0.6f;

        public override void Initialize()
        {
            Modding.ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
            Events.AddFsmEdit(SceneNames.Room_shop, new("Basement Closed", "Control"), EditBasementClosed);
            Events.AddFsmEdit(SceneNames.Room_shop, new("Basement Open", "Control"), EditBasementOpen);
            Events.AddFsmEdit(SceneNames.Room_shop, new("Shop Region", "Shop Region"), EditShopMoveToX);
            Events.AddSceneChangeEdit(SceneNames.Room_shop, EditScene);
        }

        public override void Unload()
        {
            Modding.ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Basement Closed", "Control"), EditBasementClosed);
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Basement Open", "Control"), EditBasementOpen);
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Shop Region", "Shop Region"), EditShopMoveToX);
            Events.RemoveSceneChangeEdit(SceneNames.Room_shop, EditScene);
        }

        private void EditScene(Scene scene)
        {
            GameObject counter = scene.FindGameObject("_Scenery/Shop Counter");
            if (counter == null) ItemChangerMod.instance.Log("Unable to find counter!");
            else counter.transform.Translate(new Vector2(closedOffset, 0f));
        }

        private void EditBasementOpen(PlayMakerFSM fsm)
        {
            fsm.transform.Translate(new Vector2(openOffset, 0));
        }

        private void EditBasementClosed(PlayMakerFSM fsm)
        {
            fsm.GetState("Check").ClearTransitions();
            fsm.transform.Translate(new Vector2(closedOffset, 0f));
        }

        private void EditShopMoveToX(PlayMakerFSM fsm)
        {
            fsm.FsmVariables.FindFsmFloat("Move To X").Value += closedOffset;
        }

        private bool GetPlayerBoolHook(string name, bool orig)
        {
            switch (name)
            {
                case nameof(PlayerData.gotSlyCharm):
                    return Closed;
                case nameof(PlayerData.hasAllNailArts):
                    return PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                        && PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                        && PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash));
                case nameof(PlayerData.hasNailArt):
                    return PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                        || PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                        || PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash));
                default:
                    return orig;
            }
        }
    }
}
