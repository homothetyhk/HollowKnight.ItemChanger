namespace ItemChanger.Placements
{
    /// <summary>
    /// Interface for accessing the primary location of a placement, if it has one.
    /// </summary>
    public interface IPrimaryLocationPlacement
    {
        AbstractLocation Location { get; }
    }
}
