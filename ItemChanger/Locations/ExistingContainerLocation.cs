using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    /// <summary>
    /// A location for modifying an object in-place with the specified Container.
    /// </summary>
    public class ExistingContainerLocation : AutoLocation
    {
        public string objectName;
        public string fsmName;
        public string containerType;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), OnEnable);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), OnEnable);
        }

        public void OnEnable(PlayMakerFSM fsm)
        {
            var info = fsm.gameObject.GetOrAddComponent<ContainerInfo>();
            info.containerType = containerType;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = Placement,
                items = Placement.Items,
                flingType = flingType,
            };
        }
    }
}
