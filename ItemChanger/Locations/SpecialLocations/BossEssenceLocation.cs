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
    public class BossEssenceLocation : AutoLocation
    {
        public string fsmName;
        public string objName;

        // TODO: change bool test, so that location can be checked multiple times if necessary

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objName, fsmName), EditBossConvo);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objName, fsmName), EditBossConvo);
        }

        private void EditBossConvo(PlayMakerFSM fsm)
        {
            FsmState get = fsm.GetState("Get");
            List<FsmStateAction> fsmActions = get.Actions.ToList();
            fsmActions.RemoveAt(fsmActions.Count - 1); // SendEventByName (essence counter)
            fsmActions.RemoveAt(fsmActions.Count - 1); // PlayerDataIntAdd (add essence)
            fsmActions.Add(new AsyncLambda(GiveAllAsync(fsm.transform)));
            get.Actions = fsmActions.ToArray();
        }
    }
}
