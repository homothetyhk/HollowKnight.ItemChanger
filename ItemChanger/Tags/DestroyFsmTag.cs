namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which destroys an object when an attached fsm activates.
    /// </summary>
    public class DestroyFsmTag : Tag
    {
        public FsmID id;
        public string sceneName;

        public override void Load(object parent)
        {
            if (sceneName == null) Events.AddFsmEdit(id, Destroy);
            else Events.AddFsmEdit(sceneName, id, Destroy);
        }

        public override void Unload(object parent)
        {
            if (sceneName == null) Events.RemoveFsmEdit(id, Destroy);
            else Events.RemoveFsmEdit(sceneName, id, Destroy);
        }

        private void Destroy(PlayMakerFSM fsm)
        {
            UObject.Destroy(fsm.gameObject);
        }
    }
}
