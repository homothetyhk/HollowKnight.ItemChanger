using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which prevents Zote from dying for any reason.
    /// </summary>
    [DefaultModule]
    public class PreventZoteDeath : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(new("Check Zote Death"), PreventZoteDeathCheck);
            Events.AddFsmEdit(new("Shiny Control"), PreventZoteEvent);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Check Zote Death"), PreventZoteDeathCheck);
            Events.RemoveFsmEdit(new("Shiny Control"), PreventZoteEvent);
        }

        // room triggers in deepnest and city
        private void PreventZoteDeathCheck(PlayMakerFSM fsm)
        {
            UObject.Destroy(fsm);
        }

        // mantis claw trigger
        private void PreventZoteEvent(PlayMakerFSM fsm)
        {
            fsm.GetState("Zote Event").ClearActions();
        }

    }
}
