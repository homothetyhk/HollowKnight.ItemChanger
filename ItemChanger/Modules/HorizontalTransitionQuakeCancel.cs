namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which prevents glitches from diving into a bot -> non-top transition.
    /// </summary>
    [DefaultModule]
    public class HorizontalTransitionQuakeCancel : Module
    {
        public override void Initialize()
        {
            Events.OnTransitionOverride += OnTransitionOverride;
        }

        public override void Unload()
        {
            Events.OnTransitionOverride -= OnTransitionOverride;
        }

        private void OnTransitionOverride(Transition source, Transition origTarget, ITransition newTarget)
        {
            string gate = newTarget?.GateName;
            if (gate != null && !gate.StartsWith("top"))
            {
                if (HeroController.SilentInstance?.cState?.spellQuake == true)
                {
                    HeroController.SilentInstance.cState.spellQuake = false;
                }
            }
        }
    }
}
