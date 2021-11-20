namespace ItemChanger.Items
{
    /// <summary>
    /// BoolItem which sends an event for acid pools to recheck whether the player has Isma's Tear.
    /// </summary>
    public class IsmaItem : BoolItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            base.GiveImmediate(info);
            PlayMakerFSM.BroadcastEvent("GET ACID ARMOUR");
        }
    }
}
