using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// FsmObjectLocation with edits to support dropping from the Grey Mourner and giving a hint when the Delicate Flower is offered.
    /// </summary>
    public class GreyMournerLocation : FsmObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Xun NPC", "Conversation Control"), EditXunConvo);
            Events.AddFsmEdit(sceneName, new("Heart Piece Folder", "Activate"), EditHeartPieceActivate);
            Events.AddLanguageEdit(new("Prompts", "XUN_OFFER"), OnLanguageGet);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Xun NPC", "Conversation Control"), EditXunConvo);
            Events.RemoveFsmEdit(sceneName, new("Heart Piece Folder", "Activate"), EditHeartPieceActivate);
            Events.RemoveLanguageEdit(new("Prompts", "XUN_OFFER"), OnLanguageGet);
        }

        private void EditXunConvo(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.SetActions(init.Actions.Where(a => !(a is FindChild fc) || fc.childName.Value != "Heart Piece").ToArray());

            FsmState crumble = fsm.GetState("Crumble");
            crumble.RemoveActionsOfType<SetFsmGameObject>();
        }

        private void EditHeartPieceActivate(PlayMakerFSM fsm)
        {
            FsmState activate = fsm.GetState("Activate");
            activate.RemoveActionsOfType<FindChild>();
            activate.RemoveActionsOfType<SetFsmBool>();
        }

        private void OnLanguageGet(ref string value)
        {
            if (this.GetItemHintActive())
            {
                string text = Placement.GetUIName();
                value = string.Format(Language.Language.Get("XUN_OFFER_HINT", "Fmt"), text);
                Placement.OnPreview(text);
            }
        }
    }
}
