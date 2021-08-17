using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using ItemChanger.Extensions;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    /// <summary>
    /// ObjectLocation which replaces an FsmGameObject
    /// </summary>
    public class FsmObjectLocation : ObjectLocation
    {
        public string fsmName;
        public string fsmParent;
        public string fsmVariable;

        public override void PlaceContainer(GameObject obj, string containerType)
        {
            base.PlaceContainer(obj, containerType);
            GameObject.Find(fsmParent).LocateFSM(fsmName).FsmVariables.FindFsmGameObject(fsmVariable).Value = obj;
        }
    }
}
