using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    /// <summary>
    /// ContainerLocation for dropping an item when Gruz Mother is killed.
    /// </summary>
    public class GruzMotherDropLocation : ContainerLocation
    {
        public bool removeGeo = true;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new("burster"), EditCorpseBurst);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new("burster"), EditCorpseBurst);
        }

        private void EditCorpseBurst(PlayMakerFSM fsm)
        {
            if (!fsm.gameObject.name.StartsWith("Corpse Big Fly Burster")) return;

            FsmState geo = fsm.GetState("Geo");
            if (removeGeo) geo.Actions = new FsmStateAction[0];
            geo.AddLastAction(new Lambda(() => PlaceContainer(fsm.gameObject)));
        }

        private void PlaceContainer(GameObject gruz)
        {
            base.GetContainer(out GameObject obj, out string containerType);
            Container.GetContainer(containerType).ApplyTargetContext(obj, gruz, 0);
        }
    }
}
