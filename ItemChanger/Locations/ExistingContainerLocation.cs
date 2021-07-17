using ItemChanger.Components;
using ItemChanger.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    public class ExistingContainerLocation : AutoLocation
    {
        public string objectName;
        public string fsmName;
        public string containerType;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            if (objectName == fsm.gameObject.name && fsm.FsmName == fsmName)
            {
                Transform = fsm.transform;
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
}
