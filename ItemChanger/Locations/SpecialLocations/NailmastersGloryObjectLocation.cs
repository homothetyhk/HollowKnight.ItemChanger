using ItemChanger.FsmStateActions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which places a shiny which triggers a scene change to Dirtmouth. Expects no other shinies are placed in Room_Sly_Storeroom.
    /// </summary>
    public class NailmastersGloryObjectLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(UnsafeSceneName, new("Shiny Control"), EditShiny);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(UnsafeSceneName, new("Shiny Control"), EditShiny);
        }

        private void EditShiny(PlayMakerFSM fsm)
        {
            fsm.FsmVariables.FindFsmBool("Exit Dream").Value = true;
            fsm.GetState("Fade Pause").AddFirstAction(new Lambda(() =>
            {
                PlayerData.instance.dreamReturnScene = "Town";
            }));
        }
    }
}
