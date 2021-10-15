using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using UnityEngine;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which edits Grubfather to give all unlocked rewards at once.
    /// </summary>
    [DefaultModule]
    public class FastGrubfather : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Crossroads_38, new("Grub King", "King Control"), CombineRewards);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Crossroads_38, new("Grub King", "King Control"), CombineRewards);
        }

        private void CombineRewards(PlayMakerFSM fsm)
        {
            FsmState finalReward = fsm.GetState("Final Reward?");
            FsmState recheck = fsm.GetState("Recheck");
            FsmState allGiven = fsm.GetState("All Given");
            FsmState activateReward = fsm.GetState("Activate Reward");

            finalReward.RemoveTransitionsTo("Recover");
            finalReward.AddTransition("FINISHED", recheck);
            recheck.RemoveTransitionsTo("Gift Anim");
            recheck.AddTransition("FINISHED", activateReward);

            FsmInt geoTotal = fsm.AddFsmInt("Geo Total", 0);
            allGiven.AddLastAction(new FlingGeoAction(fsm.gameObject, geoTotal, true));

            activateReward.Actions = new[]
            {
                activateReward.Actions[0], // increment PD int
                activateReward.Actions[1], // get PD int
                new ActivateNextGrubReward(),
            };
        }

        private class ActivateNextGrubReward : FsmStateAction
        {
            public override void OnEnter()
            {
                int rewardNum = Fsm.GetFsmInt("Rewards Given").Value;
                GameObject rewardsParent = Fsm.GameObject.FindChild("Rewards Parent");
                if (rewardsParent == null) return;
                GameObject grubReward = rewardsParent.FindChild($"Reward {rewardNum}");
                if (grubReward == null) return;

                if (grubReward.gameObject.LocateFSM("grub_reward_geo") is PlayMakerFSM geoFsm && geoFsm != null && geoFsm.FsmVariables.FindFsmInt("Geo") is FsmInt geoInt)
                {
                    Fsm.GetFsmInt("Geo Total").Value += geoInt.Value;
                }
                else
                {
                    Fsm.BroadcastEventToGameObject(grubReward.gameObject, "GIVE", sendToChildren: false, excludeSelf: true);
                }

                Finish();
            }
        }
    }
}
