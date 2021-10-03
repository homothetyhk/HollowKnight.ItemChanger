using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Locations.SpecialLocations
{
    public class StaticJournalEntryLocation : AutoLocation
    {
        public string objectName;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, "Conversation Control"), EditConvo);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, "Conversation Control"), EditConvo);
        }

        private void EditConvo(PlayMakerFSM fsm)
        {
            switch (sceneName)
            {
                default:
                    {
                        FsmState journal = fsm.GetState("Journal");
                        journal.Actions = new FsmStateAction[]
                        {
                            new BoolTestMod(Placement.AllObtained, journal.GetFirstActionOfType<PlayerDataBoolTest>()),
                            new AsyncLambda(GiveAllAsync(fsm.transform)),
                        };
                    }
                    break;
                case SceneNames.White_Palace_20:
                    {
                        FsmState disappearScne = fsm.GetState("Disappear Scne");
                        FsmState fadeOut = fsm.GetState("Fade Out");
                        FsmState journal = fsm.GetState("Journal");
                        journal.ClearActions();

                        FsmState give = new FsmState(journal);
                        give.ClearTransitions();
                        fsm.AddState(give);
                        disappearScne.Transitions[0].SetToState(give);
                        give.AddTransition(FsmEvent.Finished, fadeOut);
                        give.Actions = new FsmStateAction[]
                        {
                            new BoolTestMod(Placement.AllObtained, FsmEvent.Finished, null),
                            new AsyncLambda(GiveAllAsync(fsm.transform)),
                        };
                    }
                    break;
            }
        }
    }
}
