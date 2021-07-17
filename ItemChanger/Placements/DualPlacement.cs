using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class DualPlacement : AbstractPlacement, IContainerPlacement
    {
        public AbstractLocation trueLocation;
        public AbstractLocation falseLocation;

        public IBool Test;
        private bool cachedValue;

        public string containerType = Container.Unknown;
        public override string MainContainerType => containerType;

        public override AbstractLocation Location => cachedValue ? trueLocation : falseLocation;

        
        public override void OnLoad()
        {
            cachedValue = Test.Value;
            trueLocation.Placement = this;
            falseLocation.Placement = this;
            base.OnLoad();
        }

        public override void OnSceneFetched(Scene target)
        {
            cachedValue = Test.Value;
            base.OnSceneFetched(target);
        }

        // MutablePlacement implementation of GetContainer
        public void GetContainer(AbstractLocation location, out GameObject obj, out string containerType)
        {
            if (this.containerType == Container.Unknown)
            {
                this.containerType = MutablePlacement.ChooseContainerType(location as Locations.ContainerLocation, Items);
            }

            containerType = this.containerType;
            var container = Container.GetContainer(containerType);
            if (containerType == null)
            {
                ItemChanger.instance.LogError($"Unknown container type {containerType} used for {Name}!");
            }

            obj = container.GetNewContainer(this, Items, location.flingType);
        }
    }
}
