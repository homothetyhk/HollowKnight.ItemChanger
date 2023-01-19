using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which places an item inside the Godseeker's coffin and supports a hint through the coffin's inspect text.
    /// </summary>
    public class GodtunerLocation : ObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddLanguageEdit(new("CP3", "GODSEEKER_COFFIN_KEY"), ChangeCoffinText);
            Events.AddLanguageEdit(new("CP3", "GODSEEKER_COFFIN_NOKEY"), ChangeNoKeyCoffinText);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveLanguageEdit(new("CP3", "GODSEEKER_COFFIN_KEY"), ChangeCoffinText);
            Events.RemoveLanguageEdit(new("CP3", "GODSEEKER_COFFIN_NOKEY"), ChangeNoKeyCoffinText);
        }

        public override void OnActiveSceneChanged(Scene to)
        {
            GetContainer(out GameObject obj, out string containerType);
            PlaceContainer(obj, containerType);

            // the godtuner shiny reference is stored in actions, not in a variable.
            GameObject godseekerFall = to.FindGameObject("Godseeker Waterways/Godseeker Fall")!;
            PlayMakerFSM fallAnim = godseekerFall.LocateMyFSM("Fall Anim");
            fallAnim.GetState("Land Effects").GetActionsOfType<ActivateGameObject>()[2]
                .gameObject.GameObject = obj;
            GameObject coffin = to.FindGameObject("Godseeker Waterways/Coffin")!;
            PlayMakerFSM convo = coffin.LocateMyFSM("Conversation Control");
            convo.GetState("Idle").GetFirstActionOfType<ActivateGameObject>()
                .gameObject.GameObject = obj;
        }

        private void ChangeCoffinText(ref string value)
        {
            if (this.GetItemHintActive())
            {
                string text = Placement.GetUIName(40);
                value = string.Format(Language.Language.Get("GODSEEKER_COFFIN_KEY_HINT", "Fmt"), text);
                Placement.OnPreview(text);
            }
        }

        private void ChangeNoKeyCoffinText(ref string value)
        {
            if (this.GetItemHintActive())
            {
                string text = Placement.GetUIName();
                value = string.Format(Language.Language.Get("GODSEEKER_COFFIN_NOKEY_HINT", "Fmt"), text);
                Placement.OnPreview(text);
            }
        }
    }
}
