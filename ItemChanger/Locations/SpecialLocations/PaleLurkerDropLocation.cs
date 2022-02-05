namespace ItemChanger.Locations.SpecialLocations
{
    public class PaleLurkerDropLocation : EnemyLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Placement.OnVisitStateChanged += OnDropped;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Placement.OnVisitStateChanged -= OnDropped;
        }

        private void OnDropped(VisitStateChangedEventArgs obj)
        {
            if ((obj.NewFlags & VisitState.Dropped) == VisitState.Dropped)
            {
                PlayMakerFSM.BroadcastEvent("SHINY PICKED UP");
            }
        }
    }
}
