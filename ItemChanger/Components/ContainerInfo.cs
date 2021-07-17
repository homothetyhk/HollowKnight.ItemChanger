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
        public string containerType;

        public ContainerGiveInfo giveInfo;
        public ChangeSceneInfo changeSceneInfo;
        public CostInfo costInfo;
    }

    
    public class ContainerGiveInfo
    {
        public IEnumerable<AbstractItem> items;
        public AbstractPlacement placement;
        public FlingType flingType; // TODO: Should the FlingType parameter be replaced with GiveInfo?
        public bool applied;
    }

    public class ChangeSceneInfo
    {
        public const string door_dreamReturn = "door_dreamReturn";

        public string toScene;
        public string toGate = door_dreamReturn;
        public bool applied;
    }

    public class CostInfo
    {
        public Cost cost;
        public IEnumerable<AbstractItem> previewItems;
        public bool applied;
    }
}
