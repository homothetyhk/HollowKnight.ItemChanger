using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// FsmObjectLocation with text and fsm edits for spawning an item from the donation fountain.
    /// </summary>
    public class BasinFountainLocation : FsmObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
            Events.AddLanguageEdit(new("Prompts", "GEO_RELIEVE"), EditFountainText);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new(fsmParent, fsmName), EditFountain);
            Events.RemoveLanguageEdit(new("Prompts", "GEO_RELIEVE"), EditFountainText);
        }

        private void EditFountain(PlayMakerFSM fsm)
        {
            FsmState idle = fsm.GetState("Idle");
            idle.Actions = new FsmStateAction[]
            {
                    idle.Actions[0],
                    idle.Actions[1],
                    // idle.Actions[2], // FindChild -- Vessel Fragment
                    idle.Actions[3],
            };
        }

        private void EditFountainText(ref string value)
        {
            string text = Placement.GetUIName(40);
            if (value.EndsWith("?"))
            {
                value = value.Replace("?", $" for {(Placement.Items.Count > 0 ? "a " : "")}{text}?");
            }
            else
            {
                value += $" For {(Placement.Items.Count > 0 ? "a " : "")}{text}?";
            }
            Placement.OnPreview(text);
        }
    }
}
