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
            On.GameManager.BeginSceneTransition += BeginSceneTransition;
        }

        public override void Unload()
        {
            On.GameManager.BeginSceneTransition -= BeginSceneTransition;
        }

        private void BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            if (info.EntryGateName != null && !info.EntryGateName.StartsWith("top"))
            {
                if (HeroController.SilentInstance?.cState?.spellQuake == true)
                {
                    HeroController.SilentInstance.cState.spellQuake = false;
                }
            }
            orig(self, info);
        }
    }
}
