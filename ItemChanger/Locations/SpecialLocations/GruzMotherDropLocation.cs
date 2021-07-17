using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations.SpecialLocations
{
    public class GruzMotherDropLocation : ContainerLocation
    {
        public bool removeGeo = true;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "burster" && fsm.gameObject.name.StartsWith("Corpse Big Fly Burster"))
            {
                FsmState geo = fsm.GetState("Geo");
                if (removeGeo) geo.Actions = new FsmStateAction[0];
                geo.AddAction(new Lambda(() => PlaceContainer(fsm.gameObject)));
            }
        }

        private void PlaceContainer(GameObject gruz)
        {
            base.GetContainer(out GameObject obj, out string containerType);
            Container.GetContainer(containerType).ApplyTargetContext(obj, gruz, 0);
        }
    }
}
