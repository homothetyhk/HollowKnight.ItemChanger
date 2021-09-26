using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using UnityEngine.SceneManagement;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Manage whether Sly has been rescued independently of PlayerData.<br/>
    /// Then, PD controls the door to the shop, while the module controls the dazed sly encounter and prevents Sly from appearing in the shop before the encounter is complete.
    /// </summary>
    [DefaultModule]
    public class SlyRescuedEvent : Module
    {
        public bool SlyRescued { get; private set; } = false;

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

            if (active.GetFirstActionOfType<BoolTest>() is BoolTest test1)
            {
                active.AddLastAction(new Lambda(() => dazedSly.SendEvent(SlyRescued ? test1.isTrue?.Name : test1.isFalse?.Name)));
                active.RemoveActionsOfType<BoolTest>();
            }

            if (convo.GetFirstActionOfType<BoolTest>() is BoolTest test2)
            {
                active.AddLastAction(new Lambda(() => dazedSly.SendEvent(SlyRescued ? test2.isTrue?.Name : test2.isFalse?.Name)));
                active.RemoveActionsOfType<BoolTest>();
            }

            meet.AddLastAction(new Lambda(() => SlyRescued = true));
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
