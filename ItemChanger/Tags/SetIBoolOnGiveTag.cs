namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which sets an IWriteableBool when its parent item is given.
    /// <br/>If attached to a location or placement, sets the bool when VisitState.ObtainedAnyItem is first set on the placement.
    /// </summary>
    public class SetIBoolOnGiveTag : Tag
    {
        public IWritableBool Bool;
        public bool value = true;

        public override void Load(object parent)
        {
            if (parent is AbstractItem item)
            {
                item.OnGive += OnGive;
            }
            else
            {
                AbstractPlacement placement = parent as AbstractPlacement ?? (parent as AbstractLocation)?.Placement;
                if (placement is not null)
                {
                    placement.OnVisitStateChanged += OnVisitStateChanged;
                }
            }
        }

        public override void Unload(object parent)
        {
            if (parent is AbstractItem item)
            {
                item.OnGive -= OnGive;
            }
            else
            {
                AbstractPlacement placement = parent as AbstractPlacement ?? (parent as AbstractLocation)?.Placement;
                if (placement is not null)
                {
                    placement.OnVisitStateChanged -= OnVisitStateChanged;
                }
            }
        }

        private void OnGive(ReadOnlyGiveEventArgs obj)
        {
            Bool.Value = value;
        }

        private void OnVisitStateChanged(VisitStateChangedEventArgs obj)
        {
            if (obj.NewFlags.HasFlag(VisitState.ObtainedAnyItem) && !obj.Orig.HasFlag(VisitState.ObtainedAnyItem))
            {
                Bool.Value = value;
            }
        }
    }
}
