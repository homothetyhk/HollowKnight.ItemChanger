namespace ItemChanger.Items
{
    /// <summary>
    /// BoolItem which unlocks the map and map pin panels, triggers a map update, and opens Iselda.
    /// </summary>
    public class MapPinItem : AbstractItem
    {
        public string fieldName;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.hasMap), true);
            PlayerData.instance.SetBool(nameof(PlayerData.hasPin), true);
            PlayerData.instance.SetBool(nameof(PlayerData.openedMapperShop), true);
            PlayerData.instance.SetBool(fieldName, true);
            PlayMakerFSM.BroadcastEvent("NEW MAP KEY ADDED");
            GameObject.FindObjectOfType<GameMap>()?.SetupMap(pinsOnly: true);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(fieldName);
        }
    }
}
