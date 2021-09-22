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
    public class VengefulSpiritLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Shaman Meeting", "Conversation Control"), EditShamanConvo);
            Events.AddFsmEdit(sceneName, new("Shaman Trapped", "Conversation Control"), Destroy);
            Events.AddFsmEdit(sceneName, new("Bone Gate", "Bone Gate"), Destroy);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Shaman Meeting", "Conversation Control"), EditShamanConvo);
            Events.RemoveFsmEdit(sceneName, new("Shaman Trapped", "Conversation Control"), Destroy);
            Events.RemoveFsmEdit(sceneName, new("Bone Gate", "Bone Gate"), Destroy);
        }


        public override void PlaceContainer(GameObject obj, string containerType)
        {
            base.PlaceContainer(obj, containerType);
            if (PlayerData.instance.GetInt(nameof(PlayerData.shaman)) >= 1) obj.SetActive(true);
        }

        private void EditShamanConvo(PlayMakerFSM fsm)
        {
            FsmState checkActive = fsm.GetState("Check Active");
            FsmState checkSummoned = fsm.GetState("Check Summoned");
            FsmState spellAppear = fsm.GetState("Spell Appear");

            checkActive.Actions = new FsmStateAction[0];
            checkSummoned.RemoveActionsOfType<FindChild>();
            checkSummoned.GetActionsOfType<ActivateGameObject>().First(a => a.gameObject.GameObject.Name == "Vengeful Spirit").recursive = false;
            spellAppear.GetActionsOfType<ActivateGameObject>().First(a => a.gameObject.GameObject.Name == "Vengeful Spirit").recursive = false;
            spellAppear.Actions[8] = new Lambda(() => { }); // this replaces a wait after the spawn animation and seems to prevent a freeze
        }

        private void Destroy(PlayMakerFSM fsm)
        {
            UnityEngine.Object.Destroy(fsm.gameObject);
        }
    }
}
