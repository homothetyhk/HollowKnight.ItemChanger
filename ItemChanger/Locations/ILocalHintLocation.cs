namespace ItemChanger.Locations
{
    /// <summary>
    /// Interface for locations which support a nearby toggleable item preview.
    /// </summary>
    public interface ILocalHintLocation
    {
        bool HintActive { get; set; }
    }
    public static class LocalHintLocationExtensions
    {
        public static bool GetItemHintActive(this ILocalHintLocation ilhl)
        {
            if (ilhl is AbstractLocation loc && loc.Placement.HasTag<Tags.DisableItemPreviewTag>()) return false;
            return ilhl.HintActive;
        }
    }
}
