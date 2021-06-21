using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations.SpecialLocations
{
    public class PaleLurkerLocation : ObjectLocation
    {
        private static GameObject go;

        public override void PlaceContainer(GameObject obj, Container containerType)
        {
            base.PlaceContainer(obj, containerType);
            go = obj;
        }

        public override void OnEnable(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == "Shiny Control" && fsm.gameObject.transform.parent.name.StartsWith("Corpse Pale Lurker") && fsm.gameObject != go && go != null)
            {
                go.transform.SetParent(fsm.gameObject.transform.parent);
                go.transform.position = fsm.gameObject.transform.position;
                go.transform.localPosition = fsm.gameObject.transform.localPosition;
                UnityEngine.Object.Destroy(fsm.gameObject);
            }
        }
    }
}