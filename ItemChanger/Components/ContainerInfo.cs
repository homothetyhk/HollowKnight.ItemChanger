using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using SereCore;
using ItemChanger.Util;

namespace ItemChanger.Components
{
    public class ContainerInfo : MonoBehaviour
    {
        public IEnumerable<AbstractItem> items;
        public AbstractPlacement placement;
        public FlingType flingType;
        public bool applied;
    }
}
