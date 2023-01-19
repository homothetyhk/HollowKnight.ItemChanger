using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which replaces the Hunter's Journal shiny.
    /// </summary>
    public class HuntersJournalLocation : ObjectLocation
    {
        public override void OnActiveSceneChanged(Scene to)
        {
            GetContainer(out GameObject obj, out string containerType);
            PlaceContainer(obj, containerType);
            GameObject hunterEyes = to.FindGameObject("Hunter Eyes")!;

            PlayMakerFSM checkJournalPlacement = hunterEyes.LocateMyFSM("Check Journal Placement");
            checkJournalPlacement.FsmVariables.FindFsmGameObject("Shiny Item").Value = obj;
            FsmState checkJournal = checkJournalPlacement.GetState("Check Journal");
            checkJournal.ReplaceAction(new DelegateBoolTest(() => PlayerData.instance.GetBool(nameof(PlayerData.metHunter)) && !Placement.AllObtained(), "PLACE", null), 0);

            hunterEyes.LocateMyFSM("Conversation Control").FsmVariables.FindFsmGameObject("Shiny Item").Value = obj;
        }
    }
}
