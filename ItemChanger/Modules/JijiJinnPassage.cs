using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which causes Room_Ouiji[left1] to lead to Room_Jinn[left1], and Room_Jinn[left1] to lead to the target of Room_Ouiji[left1].
    /// The passage is created when entering Jiji or Jinn's hut for the first time.
    /// <br/>Independenlty, the module also has an option enabled by default to ensure the target of the Jiji door in Dirtmouth is not changed on steel soul saves.
    /// </summary>
    public class JijiJinnPassage : Module
    {
        [Newtonsoft.Json.JsonProperty]
        private bool PassageCreated = false;
        
        /// <summary>
        /// True by default. If true, the module disables the fsm which changes the target of the Jiji door in Dirtmouth on steel soul saves.
        /// </summary>
        public bool HandleSteelSoul = true;

        public override void Initialize()
        {
            Events.OnBeginSceneTransition += BuildPassage;
            Events.AddFsmEdit(SceneNames.Town, new("door_jiji", "Set Jinn"), DisableSetJinn);
        }

        public override void Unload()
        {
            Events.OnBeginSceneTransition -= BuildPassage;
            Events.RemoveFsmEdit(SceneNames.Town, new("door_jiji", "Set Jinn"), DisableSetJinn);
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

        private void DisableSetJinn(PlayMakerFSM fsm)
        {
            if (HandleSteelSoul)
            {
                fsm.GetState("Check").ClearTransitions();
            }
        }
    }
}
