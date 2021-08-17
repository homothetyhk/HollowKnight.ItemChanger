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

namespace ItemChanger.Locations.SpecialLocations
{
    public class DreamerLocation : ObjectLocation, ILocalHintLocation
    {
        public string previousScene;
        public bool HintActive { get; set; } = true;

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            obj.GetOrAddComponent<ContainerInfo>().changeSceneInfo
                = new ChangeSceneInfo { toScene = previousScene };
            base.PlaceContainer(obj, containerType);
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            base.OnActiveSceneChanged(from, to);
            if (to.name == previousScene)
            {
                switch (previousScene)
                {
                    case "Deepnest_Spider_Town":
                        {
                            GameObject herrah = ObjectLocation.FindGameObject("Dreamer Hegemol");
                            if (herrah != null)
                            {
                                GameObject.Destroy(herrah.GetComponent<DeactivateIfPlayerdataTrue>());
                                if (Placement.AllObtained()) herrah.SetActive(false);
                                else herrah.SetActive(true);
                            }
                        }
                        break;
                }
            }
        }

        public override void OnEnableGlobal(PlayMakerFSM fsm)
        {
            base.OnEnableGlobal(fsm);
            if (fsm.gameObject.scene.name == previousScene)
            {
                if (fsm.FsmName == "FSM")
                {
                    switch (fsm.gameObject.name)
                    {
                        case "Inspect Region" when previousScene == SceneNames.Fungus3_archive_02:
                        case "Dreamer Lurien":
                        case "Monomon":
                        //case "Dreamer Hegemol":
                        case "Dream Enter":
                            {
                                ReplaceCheck(fsm);
                            }
                            break;
                    }
                }

                if (fsm.FsmName == "destroy" && fsm.gameObject.name == "Dream Enter")
                {
                    ReplaceCheck(fsm);
                }

                if (HintActive && fsm.gameObject.name == "Dream Enter" && fsm.FsmName == "Control")
                {
                    HintBox.Create(fsm.transform, Placement); // TODO: Check position in each scene, especially for Monomon
                }
            }
        }

        private void ReplaceCheck(PlayMakerFSM fsm)
        {
            FsmState check = fsm.GetState("Check");
            if (check != null) // Monomon has a second, different "FSM"
            {
                check.Actions[0] = new BoolTestMod(Placement.AllObtained, (PlayerDataBoolTest)check.Actions[0]);
            }
        }
    }
}
