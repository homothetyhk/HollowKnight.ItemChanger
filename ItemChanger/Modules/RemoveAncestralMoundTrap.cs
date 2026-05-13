namespace ItemChanger.Modules;

/// <summary>
/// Destroys the bone gate. Prior to v2.1.7, this functionality was solely handled by VengefulSpiritLocation.
/// </summary>
[DefaultModule]
public class RemoveAncestralMoundTrap : Module
{
    private const string UnsafeSceneName = SceneNames.Crossroads_ShamanTemple;

    public override void Initialize()
    {
        // rem: shaman hooks intentionally not copied, to allow progressing the shaman's dialog tree when the VS location is not modified.
        Events.AddFsmEdit(UnsafeSceneName, new("Bone Gate", "Bone Gate"), Destroy);
    }

    public override void Unload()
    {
        Events.RemoveFsmEdit(UnsafeSceneName, new("Bone Gate", "Bone Gate"), Destroy);
    }

    private void Destroy(PlayMakerFSM fsm)
    {
        UObject.Destroy(fsm.gameObject);
    }
}
