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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class VengefulSpiritLocation : ObjectLocation
    {
        public override void PlaceContainer(GameObject obj, Container containerType)
        {
            base.PlaceContainer(obj, containerType);
            GameObject.Find("Shaman Meeting").LocateMyFSM("Conversation Control").FsmVariables.FindFsmGameObject("Vengeful Spirit").Value = obj;
            if (PlayerData.instance.GetInt(nameof(PlayerData.shaman)) >= 1) obj.SetActive(true);
            GameObject.Destroy(GameObject.Find("Bone Gate"));
        }

        public override void OnEnable(PlayMakerFSM fsm)
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
                        spellAppear.Actions[8] = new Lambda(() => { });
                    }
                    break;
                case "Shaman Trapped":
                    GameObject.Destroy(fsm.gameObject);
                    break;
            }
        }
    }
}
