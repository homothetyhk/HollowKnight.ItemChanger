using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which places an item within the Dreamer's dream and supports a HintBox outside the dream.
    /// </summary>
    public class DreamerLocation : ObjectLocation, ILocalHintLocation
    {
        public string previousScene;
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(previousScene, new("Dream Enter", "FSM"), ReplaceCheck);
            Events.AddFsmEdit(previousScene, new("Dream Enter", "destroy"), ReplaceCheck);
            if (previousScene == SceneNames.Ruins2_Watcher_Room)
            {
                Events.AddFsmEdit(previousScene, new("Dreamer Lurien", "FSM"), ReplaceCheck);
            }
            else if (previousScene == SceneNames.Fungus3_archive_02)
            {
                Events.AddFsmEdit(previousScene, new("Monomon", "FSM"), ReplaceCheck);
                Events.AddFsmEdit(previousScene, new("Inspect Region", "FSM"), ReplaceCheck);
            }
            else if (previousScene == SceneNames.Deepnest_Spider_Town)
            {
                Events.AddSceneChangeEdit(previousScene, HandleHerrahDeactivation);
            }
            Events.AddFsmEdit(previousScene, new("Dream Enter", "Control"), MakeHintBox);
            try
            {
                Type.GetType("QoL.SettingsOverride, QoL")
                    ?.GetMethod("OverrideSettingToggle", BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { "SkipCutscenes", "DreamersGet", false });
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError(e);
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(previousScene, new("Dream Enter", "FSM"), ReplaceCheck);
            Events.RemoveFsmEdit(previousScene, new("Dream Enter", "destroy"), ReplaceCheck);
            if (previousScene == SceneNames.Ruins2_Watcher_Room)
            {
                Events.RemoveFsmEdit(previousScene, new("Dreamer Lurien", "FSM"), ReplaceCheck);
            }
            else if (previousScene == SceneNames.Fungus3_archive_02)
            {
                Events.RemoveFsmEdit(previousScene, new("Monomon", "FSM"), ReplaceCheck);
                Events.RemoveFsmEdit(previousScene, new("Inspect Region", "FSM"), ReplaceCheck);
            }
            else if (previousScene == SceneNames.Deepnest_Spider_Town)
            {
                Events.RemoveSceneChangeEdit(previousScene, HandleHerrahDeactivation);
            }
            Events.RemoveFsmEdit(previousScene, new("Dream Enter", "Control"), MakeHintBox);
            try
            {
                Type.GetType("QoL.SettingsOverride, QoL")
                    ?.GetMethod("RemoveSettingOverride", BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { "SkipCutscenes", "DreamersGet" });
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError(e);
            }
        }

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            obj.GetOrAddComponent<ContainerInfo>().changeSceneInfo
                = new ChangeSceneInfo { toScene = previousScene };
            base.PlaceContainer(obj, containerType);
        }

        private void HandleHerrahDeactivation(Scene to)
        {
            GameObject herrah = to.FindGameObject("Dreamer Hegemol");
            if (herrah != null)
            {
                GameObject.Destroy(herrah.GetComponent<DeactivateIfPlayerdataTrue>());
                if (Placement.AllObtained()) herrah.SetActive(false);
                else herrah.SetActive(true);
            }
        }

        private void ReplaceCheck(PlayMakerFSM fsm)
        {
            FsmState check = fsm.GetState("Check");
            if (check != null) // Monomon has a second, different "FSM"
            {
                check.Actions[0] = new DelegateBoolTest(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
            }
        }

        private void MakeHintBox(PlayMakerFSM fsm)
        {
            if (HintActive) HintBox.Create(fsm.transform, Placement); // TODO: Check position in each scene, especially for Monomon
        }
    }
}
