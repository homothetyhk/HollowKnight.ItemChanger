using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    /// <summary>
    /// Placement which handles switching between two possible locations according to a test.
    /// </summary>
    public class DualPlacement : AbstractPlacement, IContainerPlacement, ISingleCostPlacement, IPrimaryLocationPlacement
    {
        public DualPlacement(string Name) : base(Name) { }

        public AbstractLocation trueLocation;
        public AbstractLocation falseLocation;

        public IBool Test;
        private bool cachedValue;

        public string containerType = Container.Unknown;
        public override string MainContainerType => containerType;

        [Newtonsoft.Json.JsonIgnore]
        public AbstractLocation Location => cachedValue ? trueLocation : falseLocation;
        
        public Cost Cost { get; set; }

        protected override void OnLoad()
        {
            cachedValue = Test.Value;
            trueLocation.Placement = this;
            falseLocation.Placement = this;
            Location.Load();
            Events.OnBeginSceneTransition += OnBeginSceneTransition;
        }

        protected override void OnUnload()
        {
            Location.Unload();
            Events.OnBeginSceneTransition -= OnBeginSceneTransition;
        }

        private void OnBeginSceneTransition(Transition obj)
        {
            bool value = Test.Value;
            if (cachedValue != value)
            {
                Location.Unload();
                cachedValue = value;
                Location.Load();
            }
        }

        // MutablePlacement implementation of GetContainer
        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = MutablePlacement.ChooseContainerType(this, location as Locations.ContainerLocation, Items);
            }

            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (containerType == null)
            {
                ItemChangerMod.instance.LogError($"Unknown container type {containerType} used for {Name}!");
            }

            obj = container.GetNewContainer(this, Items, location.flingType, Cost);
        }
    }
}
