namespace ItemChanger.Locations
{
    /// <summary>
    /// Location type which cannot accept a container, and thus must implement itself. Examples include items given in dialogue, etc.
    /// </summary>
    public abstract class AutoLocation : AbstractLocation
    {
        public virtual GiveInfo GetGiveInfo()
        {
            return new GiveInfo
            {
                FlingType = flingType,
                Callback = null,
                Container = Container.Unknown,
                MessageType = MessageType.Any,
            };
        }

        public void GiveAll()
        {
            Placement.GiveAll(GetGiveInfo());
        }

        public Action<Action> GiveAllAsync(Transform t)
        {
            GiveInfo gi = GetGiveInfo();
            gi.Transform = t;
            return (callback) => Placement.GiveAll(gi, callback);
        }

        public void GiveAll(Action callback)
        {
            Placement.GiveAll(GetGiveInfo(), callback);
        }

        [Newtonsoft.Json.JsonIgnore]
        public virtual bool SupportsCost => false;

        public override AbstractPlacement Wrap()
        {
            return new Placements.AutoPlacement(name)
            {
                Location = this,
            };
        }

    }
}
