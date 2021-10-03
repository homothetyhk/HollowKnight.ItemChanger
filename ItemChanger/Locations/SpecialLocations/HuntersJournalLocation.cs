using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class HuntersJournalLocation : ObjectLocation
    {
        public override void OnActiveSceneChanged(Scene to)
        {
            GetContainer(out GameObject obj, out string containerType);
            PlaceContainer(obj, containerType);
            GameObject hunterEyes = to.FindGameObject("Hunter Eyes");

            PlayMakerFSM checkJournalPlacement = hunterEyes.LocateFSM("Check Journal Placement");
            checkJournalPlacement.FsmVariables.FindFsmGameObject("Shiny Item").Value = obj;
            FsmState checkJournal = checkJournalPlacement.GetState("Check Journal");
            checkJournal.Actions[0] =
                new BoolTestMod(() => PlayerData.instance.GetBool(nameof(PlayerData.metHunter)) && !Placement.AllObtained(), "PLACE", null);

            hunterEyes.LocateFSM("Conversation Control").FsmVariables.FindFsmGameObject("Shiny Item").Value = obj;
        }
    }
}
