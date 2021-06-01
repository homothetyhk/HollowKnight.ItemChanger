using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;

namespace ItemChanger.Actions
{
    public class ChangeShinyIntoGeo : RandomizerAction
    {
        private readonly _ILP _ilp;
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;

        internal ChangeShinyIntoGeo(_ILP ilp, string sceneName, string objectName, string fsmName)
        {
            _ilp = ilp;
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            FsmState pdBool = fsm.GetState("PD Bool?");
            FsmState charm = fsm.GetState("Charm?");
            FsmState getCharm = fsm.GetState("Get Charm");

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.RemoveActionsOfType<StringCompare>();

            // Add our own check to stop the shiny from being grabbed twice
            pdBool.AddAction(
                new RandomizerExecuteLambda(() => fsm.SendEvent(
                    ItemChanger.instance.Settings.CheckObtained(_ilp.id) ? "COLLECTED" : null
                    )));

            // The "Charm?" state is a bad entry point for our geo spawning
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Get Charm");
            // The "Get Charm" state is a good entry point for our geo spawning
            getCharm.RemoveActionsOfType<SetPlayerDataBool>();
            getCharm.RemoveActionsOfType<IncrementPlayerDataInt>();
            getCharm.RemoveActionsOfType<SendMessage>();

            getCharm.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(_ilp)));
            getCharm.AddAction(new RandomizerAddGeo(fsm.gameObject, _ilp.item.geo));

            // Skip all the other type checks
            getCharm.ClearTransitions();
            getCharm.AddTransition("FINISHED", "Flash");
        }
    }
}
