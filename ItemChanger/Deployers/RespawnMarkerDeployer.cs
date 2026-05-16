using GlobalEnums;
using static ItemChanger.StartDef;

namespace ItemChanger.Deployers;

/// <summary>
/// Creates a respawn marker. Has no effect unless the host manually sets respawn to the new marker.
/// </summary>
public record RespawnMarkerDeployer : Deployer
{
    public MapZone MapZone { get; init; } = MapZone.NONE;
    public bool RespawnFacingRight { get; init; } = true;

    public override GameObject Instantiate()
    {
        GameObject marker = new()
        {
            name = RESPAWN_MARKER_NAME,
            tag = RESPAWN_TAG
        };
        marker.AddComponent<RespawnMarker>().respawnFacingRight = RespawnFacingRight;
        return marker;
    }

    public string RespawnMarkerName => $"{RESPAWN_MARKER_NAME}{(int)X}{(int)Y}";

    /// <summary>
    /// IBool tracking whether the respawn is set to the deployed marker. Use with <see cref="Tags.SetIBoolOnGiveTag"/> to have an item, location, or placement set respawn.
    /// </summary>
    public CurrentRespawnBool GetRespawnBool() => new()
    {
        RespawnScene = SceneName,
        RespawnMarkerName = RespawnMarkerName,
        RespawnType = 0,
        MapZone = MapZone,
        RespawnFacingRight = RespawnFacingRight,
    };
}
