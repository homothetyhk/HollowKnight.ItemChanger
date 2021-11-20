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
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Check Zote Death"), PreventZoteDeathCheck);
        }

        private void PreventZoteDeathCheck(PlayMakerFSM fsm)
        {
            UnityEngine.Object.Destroy(fsm);
        }
    }
}
