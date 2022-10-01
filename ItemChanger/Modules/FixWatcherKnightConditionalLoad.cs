namespace ItemChanger.Modules
{
    /// <summary>
    /// Default module which allows the Watcher Knight boss fight to load when entering the room from unexpected transitions and adjusts the gate trigger to allow entering the fight from the right.
    /// </summary>
    [DefaultModule]
    public class FixWatcherKnightConditionalLoad : Module
    {
        public override void Initialize()
        {
            On.SceneAdditiveLoadConditional.OnEnable += SceneAdditiveLoadConditional_OnEnable;
            Events.AddFsmEdit(SceneNames.Ruins2_03, new FsmID("Battle Control", "Battle Control"), MoveGateTrigger);
        }

        public override void Unload()
        {
            On.SceneAdditiveLoadConditional.OnEnable -= SceneAdditiveLoadConditional_OnEnable;
            Events.RemoveFsmEdit(SceneNames.Ruins2_03, new FsmID("Battle Control", "Battle Control"), MoveGateTrigger);
        }

        private void SceneAdditiveLoadConditional_OnEnable(On.SceneAdditiveLoadConditional.orig_OnEnable orig, SceneAdditiveLoadConditional self)
        {
            if (self.sceneNameToLoad.StartsWith(SceneNames.Ruins2_03))
            {
                self.doorTrigger = string.Empty;
            }
            orig(self);
        }

        private void MoveGateTrigger(PlayMakerFSM fsm)
        {
            BoxCollider2D box = fsm.GetComponent<BoxCollider2D>();
            Vector2 size = new(box.size.x - 0.6f, box.size.y);
            Vector2 offset = new(box.offset.x - 0.6f, box.offset.y);
            box.size = size;
            box.offset = offset;
        }
    }
}
