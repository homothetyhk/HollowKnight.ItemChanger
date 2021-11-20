namespace ItemChanger.Locations
{
    /// <summary>
    /// Location for giving items at the start of the scene, late enough that they appear on the UI and soul is not removed if during respawn.
    /// </summary>
    public class StartLocation : AutoLocation
    {
        public MessageType MessageType;

        protected override void OnLoad()
        {
            On.GameManager.FinishedEnteringScene += OnFinishedEnteringScene;
        }

        protected override void OnUnload()
        {
            On.GameManager.FinishedEnteringScene -= OnFinishedEnteringScene;
        }


        private void OnFinishedEnteringScene(On.GameManager.orig_FinishedEnteringScene orig, GameManager self)
        {
            orig(self);
            GiveItems();
        }

        private void GiveItems()
        {
            if (!Placement.AllObtained())
            {
                Placement.GiveAll(new GiveInfo
                {
                    MessageType = MessageType,
                    Container = "Start",
                    FlingType = flingType,
                    Transform = null,
                    Callback = null,
                });
            }
        }


        public override AbstractPlacement Wrap()
        {
            return new Placements.AutoPlacement(name)
            {
                Location = this,
            };
        }
    }
}
