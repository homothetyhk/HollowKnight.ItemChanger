using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which manages whether Sly has been rescued independently of PlayerData, and prevents Sly from appearing in the shop before being rescued. 
    /// <br />The PlayerData bool instead solely controls whether the door to the shop is unlocked.
    /// </summary>
    [DefaultModule]
    public class SlyRescuedEvent : Module
    {
        public bool SlyRescued { get; set; } = false;

        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Room_ruinhouse, new("Sly Dazed", "Conversation Control"), FixDazedSlyBool);
            Events.AddSceneChangeEdit(SceneNames.Room_shop, DestroyShop);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Room_ruinhouse, new("Sly Dazed", "Conversation Control"), FixDazedSlyBool);
            Events.RemoveSceneChangeEdit(SceneNames.Room_shop, DestroyShop);
        }

        private void FixDazedSlyBool(PlayMakerFSM dazedSly)
        {
            FsmState active = dazedSly.GetState("Active?");
            FsmState convo = dazedSly.GetState("Convo Choice");
            FsmState meet = dazedSly.GetState("Meet");
            FsmState repeat = dazedSly.GetState("Repeat");

            if (active.GetFirstActionOfType<BoolTest>() is BoolTest test1)
            {
                active.AddLastAction(new DelegateBoolTest(() => SlyRescued, test1));
                active.RemoveActionsOfType<BoolTest>();
            }

            if (convo.GetFirstActionOfType<BoolTest>() is BoolTest test2)
            {
                convo.AddLastAction(new DelegateBoolTest(() => SlyRescued, test2));
                convo.RemoveActionsOfType<BoolTest>();
            }

            meet.AddLastAction(new Lambda(() => SlyRescued = true));
            repeat.AddLastAction(new Lambda(() => SlyRescued = true));
        }

        private void DestroyShop(Scene scene)
        {
            if (!SlyRescued)
            {
                UObject.Destroy(scene.FindGameObjectByName("Sly Shop"));
                UObject.Destroy(scene.FindGameObjectByName("Shop Region"));
                UObject.Destroy(scene.FindGameObjectByName("Basement Open"));
                UObject.Destroy(scene.FindGameObjectByName("door1"));
            }
        }
    }
}
