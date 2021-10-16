using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Locations
{
    /// <summary>
    /// A location for modifying an object in-place with the specified Container.
    /// </summary>
    public class ExistingContainerLocation : AbstractLocation
    {
        public string objectName;
        public string fsmName;
        public string containerType;
        public bool nonreplaceable;
        public float elevation;

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
            if (fsm.gameObject.GetComponent<ContainerInfo>() is ContainerInfo info && info != null) return;
            if (Placement.MainContainerType == containerType || nonreplaceable || Container.GetContainer(Placement.MainContainerType) is not Container c || !c.SupportsInstantiate)
            {
                info = fsm.gameObject.AddComponent<ContainerInfo>();
                FillInfo(info);
                info.containerType = containerType;
            }
            else
            {
                GameObject obj = c.GetNewContainer(Placement, Placement.Items, flingType, (Placement as Placements.ISingleCostPlacement)?.Cost);
                c.ApplyTargetContext(obj, fsm.gameObject, elevation);
                UnityEngine.Object.Destroy(fsm.gameObject);
            }
        }

        private void FillInfo(ContainerInfo info)
        {
            info.containerType = containerType;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = Placement,
                items = Placement.Items,
                flingType = flingType,
            };

            if (Placement is Placements.ISingleCostPlacement iscp && iscp.Cost != null)
            {
                info.costInfo = new CostInfo
                {
                    placement = Placement,
                    previewItems = Placement.Items,
                    cost = iscp.Cost,
                };
            }

            Tags.ChangeSceneTag cst = GetTags<Tags.ChangeSceneTag>().Concat(Placement.GetTags<Tags.ChangeSceneTag>()).FirstOrDefault();
            if (cst != null)
            {
                info.changeSceneInfo = new()
                {
                    transition = cst.changeTo
                };
            }
        }

        public override AbstractPlacement Wrap()
        {
            return new Placements.ExistingContainerPlacement(name)
            {
                Location = this,
            };
        }
    }
}
