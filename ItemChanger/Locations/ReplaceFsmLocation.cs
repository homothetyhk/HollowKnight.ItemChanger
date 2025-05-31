namespace ItemChanger.Locations
{
    /// <summary>
    /// A container location like <see cref="ObjectLocation"/> which looks up the object it replaces by fsm.
    /// </summary>
    public class ReplaceFsmLocation : ContainerLocation
    {
        public string? objectName;
        public string fsmName;
        public float elevation;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(UnsafeSceneName, new(objectName, fsmName), OnEnable);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(UnsafeSceneName, new(objectName, fsmName), OnEnable);
        }

        public void OnEnable(PlayMakerFSM fsm)
        {
            base.GetContainer(out GameObject obj, out string containerType);
            Container c = Container.GetContainer(containerType)!;
            c.ApplyTargetContext(obj, fsm.gameObject, elevation);
            UObject.Destroy(fsm.gameObject);
        }
    }
}
