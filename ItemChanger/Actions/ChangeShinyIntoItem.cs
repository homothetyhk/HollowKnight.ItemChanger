using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using SereCore;
using UnityEngine;
using static ItemChanger.GiveItemActions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static ItemChanger.AdditiveManager;

namespace ItemChanger.Actions
{
    internal class ChangeShinyIntoItem : RandomizerAction
    {
        private readonly string _sceneName;
        private readonly string _objectName;
        private readonly string _fsmName;

        private readonly ILP _ilp;

        // BigItemDef array is meant to be for additive items
        // For example, items[0] could be vengeful spirit and items[1] would be shade soul
        public ChangeShinyIntoItem(ILP ilp, string sceneName, string objectName, string fsmName)
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
            FsmState bigItem = fsm.GetState("Big Item?");
            FsmState bigGetFlash = fsm.GetState("Big Get Flash");
            FsmState trinkFlash = fsm.GetState("Trink Flash");
            FsmState giveTrinket = fsm.GetState("Store Key");

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<StringCompare>();

            // Change pd bool test to our new bool
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.AddAction(
                new RandomizerExecuteLambda(() => fsm.SendEvent(
                    ItemChanger.instance.Settings.CheckObtained(_ilp.id) ? "COLLECTED" : null
                    )));

            
            // Charm must be preserved as the entry point for AddYNDialogueToShiny
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Big Item?");

            // Check whether next item should be normal or big popup
            bigItem.ClearTransitions();
            bigItem.AddFirstAction(new RandomizerExecuteLambda(() => bigItem.AddTransition("FINISHED", GetNextAdditiveItem(_ilp).item.type != Item.ItemType.Big ? "Trink Flash" : "Big Get Flash")));

            // normal path
            trinkFlash.ClearTransitions();
            trinkFlash.AddTransition("FINISHED", "Store Key");
            fsm.GetState("Trinket Type").ClearTransitions();
            trinkFlash.AddTransition("FINISHED", "Store Key");
            giveTrinket.RemoveActionsOfType<SetPlayerDataBool>();
            giveTrinket.AddAction(new RandomizerExecuteLambda(() => GiveItem(_ilp)));
            giveTrinket.AddFirstAction(new RandomizerExecuteLambda(
                () =>
                {
                    giveTrinket.GetActionsOfType<GetLanguageString>().First().convName = GetNextAdditiveItem(_ilp).item.nameKey;
                    giveTrinket.GetActionsOfType<SetSpriteRendererSprite>().First().sprite = GetNextAdditiveItem(_ilp).item.sprite;
                }));

            // Normal path for big items. Set bool and show the popup after the flash
            bigGetFlash.AddAction(new RandomizerExecuteLambda(() => BigItemPopup.Show(GetNextAdditiveItem(_ilp).item, fsm.gameObject, "GET ITEM MSG END")));

            // set the pickup
            bigGetFlash.AddAction(new RandomizerExecuteLambda(() => GiveItem(_ilp)));

            // Exit the fsm after the popup
            bigGetFlash.ClearTransitions();
            bigGetFlash.AddTransition("GET ITEM MSG END", "Hero Up");
            bigGetFlash.AddTransition("HERO DAMAGED", "Finish");
        }
    }
}
