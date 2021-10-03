using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class GodtunerLocation : ObjectLocation, ILocalHintLocation
    {
        public bool HintActive { get; set; } = true;

        protected override void OnLoad()
        {
            base.OnLoad();
            Events.AddLanguageEdit(new("CP3", "GODSEEKER_COFFIN_KEY"), ChangeCoffinText);
            Events.AddLanguageEdit(new("CP3", "GODSEEKER_COFFIN_NOKEY"), ChangeNoKeyCoffinText);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Events.RemoveLanguageEdit(new("CP3", "GODSEEKER_COFFIN_KEY"), ChangeCoffinText);
            Events.RemoveLanguageEdit(new("CP3", "GODSEEKER_COFFIN_NOKEY"), ChangeNoKeyCoffinText);
        }

        public override void OnActiveSceneChanged(Scene to)
        {
            GetContainer(out GameObject obj, out string containerType);
            PlaceContainer(obj, containerType);

            // the godtuner shiny reference is stored in actions, not in a variable.
            GameObject godseekerFall = to.FindGameObject("Godseeker Waterways/Godseeker Fall");
            PlayMakerFSM fallAnim = godseekerFall.LocateFSM("Fall Anim");
            fallAnim.GetState("Land Effects").GetActionsOfType<ActivateGameObject>()[2]
                .gameObject.GameObject = obj;
            GameObject coffin = to.FindGameObject("Godseeker Waterways/Coffin");
            PlayMakerFSM convo = coffin.LocateFSM("Conversation Control");
            convo.GetState("Idle").GetFirstActionOfType<ActivateGameObject>()
                .gameObject.GameObject = obj;
        }

        private void ChangeCoffinText(ref string value)
        {
            if (HintActive)
            {
                value = $"A cocoon containing {Placement.GetUIName(maxLength: 40)}.\nUse simple key?";
            }
        }

        private void ChangeNoKeyCoffinText(ref string value)
        {
            if (HintActive)
            {
                value = $"A cocoon chained with a simple lock. You can sense the {Placement.GetUIName()} inside.";
            }
        }
    }
}
