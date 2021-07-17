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
using SereCore;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class VengefulSpiritLocation : FsmObjectLocation
    {
        public override void PlaceContainer(GameObject obj, string containerType)
        {
            base.PlaceContainer(obj, containerType);

            // TODO: move to world event?
            if (PlayerData.instance.GetInt(nameof(PlayerData.shaman)) >= 1) obj.SetActive(true);
            UnityEngine.Object.Destroy(GameObject.Find("Bone Gate"));
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            switch (fsm.gameObject.name)
            {
                case "Shaman Meeting" when fsm.FsmName == "Conversation Control":
                    {
                        FsmState checkActive = fsm.GetState("Check Active");
                        FsmState checkSummoned = fsm.GetState("Check Summoned");
                        FsmState spellAppear = fsm.GetState("Spell Appear");

                        checkActive.Actions = new FsmStateAction[0];
                        checkSummoned.RemoveActionsOfType<FindChild>();
                        spellAppear.GetActionsOfType<ActivateGameObject>().First(a => a.gameObject.GameObject.Name == "Vengeful Spirit").recursive = false;
                        spellAppear.Actions[8] = new Lambda(() => { }); // this replaces a wait after the spawn animation and seems to prevent a freeze
                    }
                    break;
                case "Shaman Trapped":
                    UnityEngine.Object.Destroy(fsm.gameObject);
                    break;
            }
        }
    }
}
