using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ObjectLocation which makes a minor change to the Dream Nail sequence to prevent Unity error logs.
    /// </summary>
    public class DreamNailLocation : ObjectLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddFsmEdit(sceneName, new("Witch Control", "Control"), RemoveSetCollider);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveFsmEdit(sceneName, new("Witch Control", "Control"), RemoveSetCollider);
        }

        private void RemoveSetCollider(PlayMakerFSM fsm)
        {
            fsm.GetState("Convo Ready").RemoveActionsOfType<SetCollider>(); // not important, but prevents null ref unity logs after destroying Moth NPC object
        }
    }
}
