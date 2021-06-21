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
    public class BrummFlameLocation : FsmLocation
    {
        public override void OnEnable(PlayMakerFSM fsm, IFsmLocationActions actions)
        {
            switch (fsm.FsmName)
            {
                case "Conversation Control" when fsm.gameObject.name == "Brumm Torch NPC":
                    {
                        FsmState checkActive = fsm.GetState("Check Active");
                        FsmState convo1 = fsm.GetState("Convo 1");
                        FsmState get = fsm.GetState("Get");

                        checkActive.Actions = new FsmStateAction[] 
                        {
                            new BoolTestMod(() => IsBrummActive() && !actions.AllObtained(), (PlayerDataBoolTest)checkActive.Actions[0])
                        };
                        
                        convo1.RemoveActionsOfType<IntCompare>();

                        get.Actions = new FsmStateAction[]
                        {
                            get.Actions[6], // set Activated--not used by IC, but preserves grimmkin status if IC is disabled
                            get.Actions[14], // set gotBrummsFlame
                            new AsyncLambda(actions.Give),
                        };
                    }
                    break;
            }
        }

        public override Transform FindTransformInScene()
        {
            return ObjectLocation.FindGameObject("Brumm Torch NPC")?.transform;
        }

        private static bool IsBrummActive()
        {
            int grimmchildLevel = Ref.PD.GetInt("grimmChildLevel");
            return Ref.PD.GetBool("equippedCharm_40") && grimmchildLevel >= 3; // && !Ref.PD.GetBool("gotBrummsFlame") && grimmchildLevel < 5;
        }
    }
}
