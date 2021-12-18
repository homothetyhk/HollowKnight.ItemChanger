namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which causes Room_Ouiji[left1] to lead to Room_Jinn[left1], and Room_Jinn[left1] to lead to the target of Room_Ouiji[left1].
    /// The passage is created when entering Jiji or Jinn's hut for the first time.
    /// </summary>
    public class JijiJinnPassage : Module
    {
        [Newtonsoft.Json.JsonProperty]
        private bool PassageCreated = false;

        public override void Initialize()
        {
            Events.OnBeginSceneTransition += BuildPassage;
        }
        public override void Unload()
        {
            Events.OnBeginSceneTransition -= BuildPassage;
        }

        private void BuildPassage(Transition t)
        {
            if (PassageCreated)
            {
                return;
            }

            if (t.SceneName == SceneNames.Room_Ouiji || t.SceneName == SceneNames.Room_Jinn)
            {
                Transition jiji = new(SceneNames.Room_Ouiji, "left1");
                Transition jinn = new(SceneNames.Room_Jinn, "left1");

                if (Internal.Ref.Settings.TransitionOverrides.TryGetValue(jiji, out ITransition jijiTarget))
                {
                    Internal.Ref.Settings.TransitionOverrides[jinn] = jijiTarget;
                }
                Internal.Ref.Settings.TransitionOverrides[jiji] = jinn;
                
                if (ItemChangerMod.Modules.Get<CompletionPercentOverride>() is CompletionPercentOverride cpo)
                {
                    cpo.SetTransitionWeight(jiji, 0);
                }
                
                PassageCreated = true;
            }
        }
    }
}
